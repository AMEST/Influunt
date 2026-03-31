using System;
using System.Threading;
using System.Threading.Tasks;
using Influunt.Scheduler.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Influunt.Scheduler;

/// <summary>
/// Background service для создания индексов MongoDB для модуля планировщика.
/// Выполняется однократно при запуске приложения.
/// </summary>
public class IndexCreationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IndexCreationBackgroundService> _logger;

    public IndexCreationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<IndexCreationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Index Creation Background Service запущен.");

        try
        {
            await CreateIndexesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании индексов MongoDB");
            throw;
        }
        finally
        {
            _logger.LogInformation("Index Creation Background Service завершил работу.");
        }
    }

    /// <summary>
    /// Создает индексы для коллекций планировщика.
    /// </summary>
    private async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинаем создание индексов MongoDB для модуля планировщика...");

        // Используем скоуп для получения зависимостей с правильным временем жизни
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMongoContext>();

        await CreateJobDefinitionIndexesAsync(context, cancellationToken);
        await CreateJobTriggerIndexesAsync(context, cancellationToken);
        await CreateJobExecutionIndexesAsync(context, cancellationToken);

        _logger.LogInformation("Создание индексов MongoDB для модуля планировщика завершено.");
    }

    private async Task CreateJobDefinitionIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobDefinition>(Constants.JobDefinitionsCollectionName);
        
        // Уникальный индекс по имени задания
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(
                Builders<JobDefinition>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан уникальный индекс JobDefinition.Name");

        // Уникальный индекс по типу задания
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(
                Builders<JobDefinition>.IndexKeys.Ascending(x => x.JobType),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан уникальный индекс JobDefinition.JobType");
    }

    private async Task CreateJobTriggerIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobTrigger>(Constants.JobTriggersCollectionName);
        
        // Индекс для поиска триггеров по следующему времени запуска
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(
                Builders<JobTrigger>.IndexKeys.Ascending(x => x.NextRunTime),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан индекс JobTrigger.NextRunTime");

        // Уникальный индекс по идентификатору триггера
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(
                Builders<JobTrigger>.IndexKeys.Ascending(x => x.TriggerId),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан уникальный индекс JobTrigger.TriggerId");
    }

    private async Task CreateJobExecutionIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobExecution>(Constants.JobExecutionsCollectionName);
        
        // Уникальный индекс по идентификатору триггера (чтобы только одно выполнение на триггер)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.TriggerId),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан уникальный индекс JobExecution.TriggerId");

        // Индекс для поиска выполняющихся заданий по блокировке
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.LockId).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан составной индекс JobExecution.LockId+Status");

        // Индекс по типу задания и статусу
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.JobType).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан составной индекс JobExecution.JobType+Status");

        // Индекс для поиска выполнений по триггеру и статусу (используется в ScheduleJobAsync)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.TriggerId).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан составной индекс JobExecution.TriggerId+Status");

        // Индекс для поиска устаревших блокировок (используется в CreateAndStartJobExecution и TryCaptureExistingExecutionLock)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.LockExpiry),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Создан индекс JobExecution.LockExpiry");
    }
}