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
    private readonly IMongoContext _context;
    private readonly string _instanceId; // unique scheduler instance identifier
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _runningJobs = new();
    private Timer _scanTimer;
    private Timer _heartbeatTimer;
    private bool _disposed;

    public JobScheduler(IServiceProvider serviceProvider, IOptions<JobSchedulerOptions> options, ILogger<JobScheduler> logger, IMongoContext context)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
        _context = context;

        _instanceId = $"{Environment.MachineName}_{Guid.NewGuid():N}";
    }

    private IMongoCollection<JobDefinition> _jobDefinitions => _context.GetCollection<JobDefinition>(Constants.JobDefinitionsCollectionName);
    private IMongoCollection<JobTrigger> _triggers => _context.GetCollection<JobTrigger>(Constants.JobTriggersCollectionName);
    private IMongoCollection<JobExecution> _executions => _context.GetCollection<JobExecution>(Constants.JobExecutionsCollectionName);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobScheduler starting. InstanceId: {InstanceId}", _instanceId);

        // Start timers
        _scanTimer = new Timer(ScanTriggers, null, TimeSpan.Zero, TimeSpan.FromSeconds(_options.ScanIntervalSeconds));
        _heartbeatTimer = new Timer(Heartbeat, null, TimeSpan.FromSeconds(_options.HeartbeatIntervalSeconds), TimeSpan.FromSeconds(_options.HeartbeatIntervalSeconds));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("JobScheduler stopping...");

        _scanTimer?.Change(Timeout.Infinite, 0);
        _heartbeatTimer?.Change(Timeout.Infinite, 0);

        // Cancel all running jobs
        foreach (var cts in _runningJobs.Values)
            cts.Cancel();

        // Wait for all jobs to complete (simplified)
        // In a real project, more careful waiting for completion is needed
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
            JobType = registeredJob.JobType,
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
            JobType = registeredJob.JobType,
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
            JobType = registeredJob.JobType,
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
            JobType = registeredJob.JobType,
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

    // ============================== Trigger Scanning ==============================

    private async void ScanTriggers(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            // Find triggers where NextRunTime <= now and are active
            var filter = Builders<JobTrigger>.Filter.And(
                Builders<JobTrigger>.Filter.Lte(t => t.NextRunTime, now));
            var triggers = await _triggers.Find(filter).ToListAsync();

            // Check current number of running jobs
            var runningCount = _runningJobs.Count;
            
            // If limit is already reached, skip all triggers
            if (runningCount >= _options.MaxConcurrentJobs)
            {
                _logger.LogDebug("Concurrency limit reached ({Running}/{Max}). Skipping all triggers in this scan cycle.", 
                    runningCount, _options.MaxConcurrentJobs);
                return;
            }

            // Calculate how many more jobs can be started
            var availableSlots = _options.MaxConcurrentJobs - runningCount;
            var processedTriggers = 0;

            foreach (var trigger in triggers)
            {
                // If maximum possible number of triggers have been processed, exit
                if (processedTriggers >= availableSlots)
                {
                    _logger.LogDebug("Available slots exhausted. Processed {Processed}/{Available} triggers in this cycle.", 
                        processedTriggers, availableSlots);
                    break;
                }

                try
                {
                    // For each trigger, attempt to create execution
                    await ScheduleJobAsync(trigger);
                    processedTriggers++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scheduling job for trigger {TriggerId}", trigger.TriggerId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning triggers");
        }
    }

    private async Task ScheduleJobAsync(JobTrigger trigger)
    {
        // 1. Get JobDefinition to determine if the same trigger can be executed concurrently for a job
        var jobDef = await _jobDefinitions.Find(d => d.JobType == trigger.JobType).FirstOrDefaultAsync();
        if (jobDef == null)
        {
            _logger.LogWarning("Job definition not found for trigger {TriggerId}", trigger.Id);
            return;
        }

        // 2. Check if there is a lock for the current TRIGGER
        var existingExecutionFilter = Builders<JobExecution>.Filter.And(
            Builders<JobExecution>.Filter.Eq(e => e.TriggerId, trigger.TriggerId),
            Builders<JobExecution>.Filter.Eq(e => e.Status, ExecutionStatus.Running)
        );
        var existingExecution = await _executions.Find(existingExecutionFilter).FirstOrDefaultAsync();

        if (existingExecution == null)
        {
            // 3. If JobExecution doesn't exist, create it, attempt to acquire lock, start the job
            await CreateAndStartJobExecution(trigger, jobDef);
            return;
        }

        // Trigger is already running, check the time
        var now = DateTime.UtcNow;
        
        if (trigger.IsRecurring)
        {
            // 2.2 If job is recurring, check if jobExecution belongs to the current run period
            // nextRunTime is approximately equal to JobExecution startTime (epsilon time of 30 seconds)
            var epsilon = TimeSpan.FromSeconds(30);
            var timeDiff = Math.Abs((trigger.NextRunTime - existingExecution.StartTime).TotalSeconds);
            
            if (timeDiff <= epsilon.TotalSeconds)
            {
                _logger.LogDebug("Recurring job '{JobName}' trigger {TriggerId} is already running for current period. Skipping.", 
                    jobDef.Name, trigger.TriggerId);
                return;
            }
        }
        else
        {
            // 2.1 If jobs are one-time, behavior should be like with AllowConcurrentExecution=false
            // Don't allow to start, but only if this lock is not expired
            // (another replica could have died and the lock is long expired)
            if (existingExecution.LockExpiry > now)
            {
                _logger.LogDebug("One-time job '{JobName}' trigger {TriggerId} is already running and lock is not expired. Skipping.", 
                    jobDef.Name, trigger.TriggerId);
                return;
            }
        }
        
        // If lock is expired (for one-time) or for new period (for recurring),
        // attempt to capture lock of existing JobExecution
        await TryCaptureExistingExecutionLock(trigger, jobDef, existingExecution);
    }

    private async Task CreateAndStartJobExecution(JobTrigger trigger, JobDefinition jobDef)
    {
        // Create new execution record
        var execution = new JobExecution
        {
            TriggerId = trigger.TriggerId,
            JobType = trigger.JobType,
            Status = ExecutionStatus.Pending,
            StartTime = DateTime.UtcNow,
            LockId = null,
            LockExpiry = DateTime.UtcNow.AddSeconds(_options.LockTimeoutSeconds),
            LastHeartbeat = DateTime.UtcNow
        };

        try
        {
            await _executions.InsertOneAsync(execution);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey || ex.WriteError?.Code == 11000)
        {
            // Duplicate key - another replica already created JobExecution for this trigger
            _logger.LogDebug("Duplicate JobExecution for trigger {TriggerId} detected. Another replica already created it.", trigger.TriggerId);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating JobExecution for trigger {TriggerId}", trigger.TriggerId);
            return;
        }

        await TryCaptureExistingExecutionLock(trigger, jobDef, execution, ExecutionStatus.Pending);
    }

    private async Task TryCaptureExistingExecutionLock(JobTrigger trigger, JobDefinition jobDef, JobExecution existingExecution, ExecutionStatus expectedExecutionStatus = ExecutionStatus.Running )
    {
        var lockExpiry = DateTime.UtcNow.AddSeconds(_options.LockTimeoutSeconds);
        
        // Attempt to capture lock of existing JobExecution
        var filter = Builders<JobExecution>.Filter.And(
            Builders<JobExecution>.Filter.Eq(e => e.Id, existingExecution.Id),
            Builders<JobExecution>.Filter.Eq(e => e.Status, expectedExecutionStatus),
            Builders<JobExecution>.Filter.Or(
                Builders<JobExecution>.Filter.Eq(e => e.LockId, null),
                Builders<JobExecution>.Filter.Lt(e => e.LockExpiry, DateTime.UtcNow)
            )
        );
        
        var update = Builders<JobExecution>.Update
            .Set(e => e.LockId, _instanceId)
            .Set(e => e.LockExpiry, lockExpiry)
            .Set(e => e.LastHeartbeat, DateTime.UtcNow);

        var updated = await _executions.FindOneAndUpdateAsync(filter, update);
        if (updated == null)
        {
            // If failed to acquire lock, skip the trigger
            _logger.LogDebug("Failed to acquire lock for existing execution of trigger {TriggerId}", trigger.TriggerId);
            return;
        }

        // If lock acquisition succeeded, start the job
        _logger.LogInformation("Starting job '{JobName}' with existing execution. TriggerId: {TriggerId}. ExecutionId: {ExecutionId}", 
            jobDef.Name, trigger.TriggerId, existingExecution.Id);
        
        var cts = new CancellationTokenSource();
        _runningJobs.TryAdd(existingExecution.Id, cts);

        // Start asynchronous execution
        _ = Task.Run(() => ExecuteJobAsync(trigger, existingExecution, jobDef, cts.Token));
    }

    private async Task ExecuteJobAsync(JobTrigger trigger, JobExecution execution, JobDefinition jobDef, CancellationToken cancellationToken)
    {
        try
        {
            // Get job instance from DI (transient or scoped)
            using var scope = _serviceProvider.CreateScope();
            var jobType = Type.GetType(jobDef.JobType) ?? throw new InvalidOperationException($"Cannot load job type {jobDef.JobType}");

            // Check if there are settings
            if (string.IsNullOrEmpty(jobDef.SettingsType))
            {
                // Job without settings
                var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;
                if (job == null)
                    throw new InvalidOperationException($"Job {jobDef.JobType} does not implement IJob");
                await job.ExecuteAsync(cancellationToken);
            }
            else
            {
                // Job with settings
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

            // Successful completion
            await UpdateExecutionStatus(execution.Id, ExecutionStatus.Completed, result: "Success");
            // Update next run time for trigger
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
        }
    }

    private async Task UpdateExecutionStatus(string executionId, ExecutionStatus status, string result)
    {
        var update = Builders<JobExecution>.Update
            .Set(e => e.Status, status)
            .Set(e => e.EndTime, DateTime.UtcNow)
            .Set(e => e.Result, result)
            .Unset(e => e.LockId) // remove lock
            .Unset(e => e.LockExpiry);
        await _executions.UpdateOneAsync(e => e.Id == executionId, update);
    }

    private async Task UpdateTriggerNextRun(JobTrigger trigger)
    {
        if (!trigger.IsRecurring || string.IsNullOrEmpty(trigger.CronExpression))
        {
            // One-time trigger — delete it
            await _triggers.DeleteOneAsync(t => t.Id == trigger.Id);
            return;
        }

        // Calculate next run time based on Cron
        var nextRun = CronHelper.GetNextOccurrence(trigger.CronExpression, DateTime.UtcNow);
        if (!nextRun.HasValue)
        {
            // Cron will no longer fire (e.g., expired) — delete it
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

            // Find all executions that belong to this instance and are in Running status
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

    public void Dispose()
    {
        if (_disposed) return;
        _scanTimer?.Dispose();
        _heartbeatTimer?.Dispose();
        _disposed = true;
    }
}