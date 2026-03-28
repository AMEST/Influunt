using System.Collections.Concurrent;
using System.Text.Json;
using Influunt.Scheduler.Abstractions.Models;
using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Influunt.Scheduler;

 internal class JobScheduler : IScheduler, IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobScheduler> _logger;
    private readonly JobSchedulerOptions _options;
    private readonly MongoUrl _url;
    private readonly Lazy<IMongoClient> _mongoClient;
    private IMongoDatabase _database;
    private readonly string _instanceId; // уникальный идентификатор экземпляра планировщика
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _runningJobs = new();
    private Timer _scanTimer;
    private Timer _heartbeatTimer;
    private SemaphoreSlim _concurrencySemaphore;
    private bool _disposed;

    public JobScheduler(IServiceProvider serviceProvider, IOptions<JobSchedulerOptions> options, ILogger<JobScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;

        _url = MongoUrl.Create(_options.ConnectionString);
        _mongoClient = new Lazy<IMongoClient>(() => new MongoClient(_url));

        _instanceId = $"{Environment.MachineName}_{Guid.NewGuid():N}";
        _concurrencySemaphore = new SemaphoreSlim(_options.MaxConcurrentJobs, _options.MaxConcurrentJobs);
    }

    private  IMongoDatabase Database => _database ??= _mongoClient.Value.GetDatabase(_url.DatabaseName);
    private IMongoCollection<JobDefinition> _jobDefinitions => Database.GetCollection<JobDefinition>("job_definitions");
    private IMongoCollection<JobTrigger> _triggers => Database.GetCollection<JobTrigger>("job_triggers");
    private IMongoCollection<JobExecution> _executions => Database.GetCollection<JobExecution>("job_executions");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobScheduler starting. InstanceId: {InstanceId}", _instanceId);

        // Запускаем создание индексов
        Task.Factory.StartNew(CreateIndexesAsync, TaskCreationOptions.LongRunning);
        // Запускаем таймеры
        _scanTimer = new Timer(ScanTriggers, null, TimeSpan.Zero, TimeSpan.FromSeconds(_options.ScanIntervalSeconds));
        _heartbeatTimer = new Timer(Heartbeat, null, TimeSpan.FromSeconds(_options.HeartbeatIntervalSeconds), TimeSpan.FromSeconds(_options.HeartbeatIntervalSeconds));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobScheduler stopping...");

        _scanTimer?.Change(Timeout.Infinite, 0);
        _heartbeatTimer?.Change(Timeout.Infinite, 0);

        // Отменяем все запущенные задания
        foreach (var cts in _runningJobs.Values)
            cts.Cancel();

        // Ждём завершения всех заданий (упрощённо)
        // В реальном проекте нужно более тщательно ожидать завершения
        return Task.CompletedTask;
    }

    public async Task RegisterJobAsync<TJob, TSettings>(string name, string? description = null, bool allowConcurrentExecution = false)
        where TJob : IJob<TSettings>
    {
        var jobDef = new JobDefinition
        {
            Name = name,
            Description = description,
            JobType = typeof(TJob)!.AssemblyQualifiedName!,
            SettingsType = typeof(TSettings)!.AssemblyQualifiedName!,
            AllowConcurrentExecution = allowConcurrentExecution
        };
        await _jobDefinitions.ReplaceOneAsync(d => d.Name == name, jobDef, new ReplaceOptions { IsUpsert = true });
    }

    public async Task RegisterJobAsync<TJob>(string name, string? description = null, bool allowConcurrentExecution = false)
        where TJob : IJob
    {
        var jobDef = new JobDefinition
        {
            Name = name,
            Description = description,
            JobType = typeof(TJob)!.AssemblyQualifiedName!,
            AllowConcurrentExecution = allowConcurrentExecution
        };
        await _jobDefinitions.ReplaceOneAsync(d => d.Name == name, jobDef, new ReplaceOptions { IsUpsert = true });
    }

    public async Task<string> ScheduleOneTimeJobAsync<TJob>(DateTime? runAt = null, string? triggerId = null) where TJob : IJob
    {
        var cursor = await _jobDefinitions.FindAsync<JobDefinition>(x => x.JobType == typeof(TJob).AssemblyQualifiedName);
        var registeredJob = await cursor.FirstOrDefaultAsync();
        if (registeredJob is null)
            throw new InvalidOperationException("Job not registered");
        
        var trigger = new JobTrigger
        {
            TriggerId = string.IsNullOrEmpty(triggerId) ? Guid.NewGuid().ToString() : triggerId,
            NextRunTime = runAt.HasValue ? runAt.Value.ToUniversalTime() : DateTime.UtcNow
        };
        await _triggers.InsertOneAsync(trigger);
        return trigger.TriggerId;
    }

    public async Task<string> ScheduleRecurringJobAsync<TJob>(string cronExpression, string? triggerId = null) where TJob : IJob
    {
        var cursor = await _jobDefinitions.FindAsync<JobDefinition>(x => x.JobType == typeof(TJob).AssemblyQualifiedName);
        var registeredJob = await cursor.FirstOrDefaultAsync();
        if (registeredJob is null)
            throw new InvalidOperationException("Job not registered");

        var nextRun = CronHelper.GetNextOccurrence(cronExpression, DateTime.UtcNow);
        if (!nextRun.HasValue)
            throw new ArgumentException("Cron expression does not produce any future occurrence");

        var trigger = new JobTrigger
        {
            TriggerId = string.IsNullOrEmpty(triggerId) ? Guid.NewGuid().ToString() : triggerId,
            NextRunTime = nextRun.Value,
            CronExpression = cronExpression
        };
        await _triggers.InsertOneAsync(trigger);
        return trigger.TriggerId;
    }

    public async Task<string> ScheduleOneTimeJobAsync<TJob, TSettings>(TSettings settings, DateTime? runAt = null, string? triggerId = null) where TJob : IJob<TSettings>
    {
        var cursor = await _jobDefinitions.FindAsync<JobDefinition>(x => x.JobType == typeof(TJob).AssemblyQualifiedName);
        var registeredJob = await cursor.FirstOrDefaultAsync();
        if (registeredJob is null)
            throw new InvalidOperationException("Job not registered");
        if (registeredJob.SettingsType != typeof(TSettings).AssemblyQualifiedName)
            throw new InvalidOperationException("The task settings type does not match the registered type.");
        
        var trigger = new JobTrigger
        {
            TriggerId = string.IsNullOrEmpty(triggerId) ? Guid.NewGuid().ToString() : triggerId,
            NextRunTime = runAt.HasValue ? runAt.Value.ToUniversalTime() : DateTime.UtcNow,
            SettingsJson = JsonSerializer.Serialize(settings)
        };
        await _triggers.InsertOneAsync(trigger);
        return trigger.TriggerId;
    }

    public async Task<string> ScheduleRecurringJobAsync<TJob, TSettings>(string cronExpression, TSettings settings, string? triggerId = null) where TJob : IJob<TSettings>
    {
        var cursor = await _jobDefinitions.FindAsync<JobDefinition>(x => x.JobType == typeof(TJob).AssemblyQualifiedName);
        var registeredJob = await cursor.FirstOrDefaultAsync();
        if (registeredJob is null)
            throw new InvalidOperationException("Job not registered");
        if (registeredJob.SettingsType != typeof(TSettings).AssemblyQualifiedName)
            throw new InvalidOperationException("The task settings type does not match the registered type.");

        var nextRun = CronHelper.GetNextOccurrence(cronExpression, DateTime.UtcNow);
        if (!nextRun.HasValue)
            throw new ArgumentException("Cron expression does not produce any future occurrence");
        
        var trigger = new JobTrigger
        {
            TriggerId = string.IsNullOrEmpty(triggerId) ? Guid.NewGuid().ToString() : triggerId,
            NextRunTime = nextRun.Value,
            CronExpression = cronExpression,
            SettingsJson = JsonSerializer.Serialize(settings)
        };
        await _triggers.InsertOneAsync(trigger);
        return trigger.TriggerId;
    }

    public async Task UnScheduleJobAsync(string triggerId)
    {
        await _triggers.DeleteOneAsync(x => x.TriggerId == triggerId);
    }

    public async Task<IEnumerable<JobDefinition>> GetRegisteredJobsAsync()
    {
        var cursor = await _jobDefinitions.FindAsync<JobDefinition>(x => true);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<JobTrigger>> GetScheduledTriggersAsync()
    {
        var cursor = await _triggers.FindAsync<JobTrigger>(x => true);
        return await cursor.ToListAsync();
    }

    // ============================== Сканирование триггеров ==============================

    private async void ScanTriggers(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            // Находим триггеры, у которых NextRunTime <= now и которые активны
            var filter = Builders<JobTrigger>.Filter.And(
                Builders<JobTrigger>.Filter.Lte(t => t.NextRunTime, now));
            var triggers = await _triggers.Find(filter).ToListAsync();

            foreach (var trigger in triggers)
            {
                // Для каждого триггера пытаемся создать выполнение
                await ScheduleJobAsync(trigger);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning triggers");
        }
    }

    private async Task ScheduleJobAsync(JobTrigger trigger)
    {
        // Проверяем, не запущено ли уже задание с такой же блокировкой (если запрещён конкурентный запуск)
        var jobDef = await _jobDefinitions.Find(d => d.JobType == trigger.JobType).FirstOrDefaultAsync();
        if (jobDef == null)
        {
            _logger.LogWarning("Job definition not found for trigger {TriggerId}", trigger.Id);
            return;
        }

        // Если конкурентный запуск запрещён, проверяем наличие выполняющихся экземпляров
        if (!jobDef.AllowConcurrentExecution)
        {
            var runningFilter = Builders<JobExecution>.Filter.And(
                Builders<JobExecution>.Filter.Eq(e => e.JobType, trigger.JobType),
                Builders<JobExecution>.Filter.Eq(e => e.Status, ExecutionStatus.Running));
            var runningCount = await _executions.CountDocumentsAsync(runningFilter);
            if (runningCount > 0)
            {
                _logger.LogDebug("Job '{JobName}' with type '{JobType}' is already running and concurrent execution is disabled. Can't run trigger {TriggerId}",
                jobDef.Name, jobDef.JobType, trigger.TriggerId );
                return;
            }
        }

        // TODO: Надо решить проблему зависшего триггера
        // Создаём запись выполнения с блокировкой (атомарно)
        var execution = new JobExecution
        {
            TriggerId = trigger.TriggerId,
            JobType = trigger.JobType,
            Status = ExecutionStatus.Pending,
            StartTime = DateTime.UtcNow,
            LockId = null,
            LockExpiry = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow
        };

        await _executions.InsertOneAsync(execution);

        // Пытаемся вставить документ и сразу захватить блокировку
        // Используем FindOneAndUpdate с условием, что блокировка ещё не получена
        var lockExpiry = DateTime.UtcNow.AddSeconds(_options.LockTimeoutSeconds);
        var filter = Builders<JobExecution>.Filter.And(
            Builders<JobExecution>.Filter.Eq(e => e.Id, execution.Id), // только что созданный Id
            Builders<JobExecution>.Filter.Eq(e => e.Status, ExecutionStatus.Pending),
            Builders<JobExecution>.Filter.Or(
                Builders<JobExecution>.Filter.Eq(e => e.LockId, null),
                Builders<JobExecution>.Filter.Lt(e => e.LockExpiry, DateTime.UtcNow)
            )
        );
        var update = Builders<JobExecution>.Update
            .Set(e => e.Status, ExecutionStatus.Running)
            .Set(e => e.LockId, _instanceId)
            .Set(e => e.LockExpiry, lockExpiry)
            .Set(e => e.LastHeartbeat, DateTime.UtcNow);

        var updated = await _executions.FindOneAndUpdateAsync(filter, update);
        if (updated == null)
        {
            // Не удалось захватить блокировку — удаляем запись или оставляем как есть (другая реплика уже взяла)
            await _executions.DeleteOneAsync(e => e.Id == execution.Id);
            _logger.LogDebug("Failed to acquire lock for trigger {TriggerId}", trigger.TriggerId);
            return;
        }

        // Блокировка получена — запускаем выполнение
        _logger.LogInformation("Starting job {JobName}. TriggerId: {TriggerId}. ExecutionId: {ExecutionId}", jobDef.Name, trigger.TriggerId, execution.Id);
        var cts = new CancellationTokenSource();
        _runningJobs.TryAdd(execution.Id, cts);

        // Запускаем асинхронное выполнение
        _ = Task.Run(() => ExecuteJobAsync(trigger, execution, jobDef, cts.Token));
    }

    private async Task ExecuteJobAsync(JobTrigger trigger, JobExecution execution, JobDefinition jobDef, CancellationToken cancellationToken)
    {
        try
        {
            // Получаем экземпляр задания из DI (transient или scoped)
            using var scope = _serviceProvider.CreateScope();
            var jobType = Type.GetType(jobDef.JobType) ?? throw new InvalidOperationException($"Cannot load job type {jobDef.JobType}");

            // Проверяем, есть ли настройки
            if (string.IsNullOrEmpty(jobDef.SettingsType))
            {
                // Задание без настроек
                var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;
                if (job == null)
                    throw new InvalidOperationException($"Job {jobDef.JobType} does not implement IJob");
                await job.ExecuteAsync(cancellationToken);
            }
            else
            {
                // Задание с настройками
                var settingsType = Type.GetType(jobDef.SettingsType);
                if (settingsType == null)
                    throw new InvalidOperationException($"Cannot load settings type {jobDef.SettingsType}");
            
                var settings = string.IsNullOrEmpty(trigger.SettingsJson)
                    ? Activator.CreateInstance(settingsType)
                    : JsonSerializer.Deserialize(trigger.SettingsJson, settingsType);

                var jobTypeGeneric = typeof(IJob<>).MakeGenericType(settingsType);
                var job = scope.ServiceProvider.GetRequiredService(jobType);
                var method = jobType.GetMethod("ExecuteAsync");
                if (method == null)
                    throw new InvalidOperationException($"ExecuteAsync method not found on {jobDef.JobType}");
                await (Task)method.Invoke(job, [settings, cancellationToken]);
            }

            // Успешное завершение
            await UpdateExecutionStatus(execution.Id, ExecutionStatus.Completed, result: "Success");
            // Обновляем время следующего запуска триггера
            await UpdateTriggerNextRun(trigger);
            _logger.LogInformation("Job {JobName} (ExecutionId: {ExecutionId}) completed successfully", jobDef.Name, execution.Id);
        }
        catch (OperationCanceledException)
        {
            await UpdateExecutionStatus(execution.Id, ExecutionStatus.Failed, result: "Cancelled");
            _logger.LogWarning("Job {JobName} (TriggerId: {TriggerId}; ExecutionId: {ExecutionId}) was cancelled", jobDef.Name, trigger.TriggerId, execution.Id);
        }
        catch (Exception ex)
        {
            await UpdateExecutionStatus(execution.Id, ExecutionStatus.Failed, result: ex.ToString());
            _logger.LogError(ex, "Job {JobName} (TriggerId: {TriggerId}; ExecutionId: {ExecutionId}) failed", jobDef.Name, trigger.TriggerId, execution.Id);
        }
        finally
        {
            _runningJobs.TryRemove(execution.Id, out _);
            _concurrencySemaphore.Release();
        }
    }

    private async Task UpdateExecutionStatus(string executionId, ExecutionStatus status, string result)
    {
        var update = Builders<JobExecution>.Update
            .Set(e => e.Status, status)
            .Set(e => e.EndTime, DateTime.UtcNow)
            .Set(e => e.Result, result)
            .Unset(e => e.LockId) // снимаем блокировку
            .Unset(e => e.LockExpiry);
        await _executions.UpdateOneAsync(e => e.Id == executionId, update);
    }

    private async Task UpdateTriggerNextRun(JobTrigger trigger)
    {
        if (!trigger.IsRecurring || string.IsNullOrEmpty(trigger.CronExpression))
        {
            // Одноразовый триггер — удаляем
            await _triggers.DeleteOneAsync(t => t.Id == trigger.Id);
            return;
        }

        // Вычисляем следующее время запуска на основе Cron
        var nextRun = CronHelper.GetNextOccurrence(trigger.CronExpression, DateTime.UtcNow);
        if (!nextRun.HasValue)
        {
            // Cron больше не сработает (например, истёк) — удаляем
            await _triggers.DeleteOneAsync(t => t.Id == trigger.Id);
            return;
        }

        await _triggers.UpdateOneAsync(t => t.Id == trigger.Id,
            Builders<JobTrigger>.Update.Set(t => t.NextRunTime, nextRun.Value));
    }

    // ============================== Heartbeat ==============================

    private async void Heartbeat(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var newExpiry = now.AddSeconds(_options.LockTimeoutSeconds);

            // Находим все выполнения, которые принадлежат этому экземпляру и находятся в статусе Running
            var filter = Builders<JobExecution>.Filter.And(
                Builders<JobExecution>.Filter.Eq(e => e.LockId, _instanceId),
                Builders<JobExecution>.Filter.Eq(e => e.Status, ExecutionStatus.Running));

            var update = Builders<JobExecution>.Update
                .Set(e => e.LockExpiry, newExpiry)
                .Set(e => e.LastHeartbeat, now);

            var result = await _executions.UpdateManyAsync(filter, update);
            _logger.LogDebug("Heartbeat: updated {Count} executions", result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during heartbeat");
        }
    }

    private async Task CreateIndexesAsync()
    {
        // Индекс по имени задания
        await _jobDefinitions.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(Builders<JobDefinition>.IndexKeys.Ascending(x => x.Name),
            new CreateIndexOptions { Unique = true, Background = true }));
        // Индекс по типу задания
        await _jobDefinitions.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(Builders<JobDefinition>.IndexKeys.Ascending(x => x.JobType),
            new CreateIndexOptions { Unique = true, Background = true }));

        // Индекс для поиска триггеров по следующему времени запуска
        await _triggers.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(Builders<JobTrigger>.IndexKeys.Ascending(x => x.NextRunTime),
            new CreateIndexOptions { Background = true }));

        // Индекс поиска триггера по JobName
        await _triggers.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(Builders<JobTrigger>.IndexKeys.Ascending(x => x.TriggerId), 
            new CreateIndexOptions { Unique = true, Background = true }));

        // Индекс для поиска выполняющихся заданий по блокировке
        await _executions.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(Builders<JobExecution>.IndexKeys.Ascending(x => x.LockId).Ascending(x => x.Status),
            new CreateIndexOptions { Background = true }));
        await _executions.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(Builders<JobExecution>.IndexKeys.Ascending(x => x.JobType).Ascending(x => x.Status),
            new CreateIndexOptions { Background = true }));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _scanTimer?.Dispose();
        _heartbeatTimer?.Dispose();
        _concurrencySemaphore?.Dispose();
        _disposed = true;
    }
}