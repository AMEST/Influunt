using Influunt.Scheduler.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Influunt.Scheduler;

/// <summary>
/// Background service for creating MongoDB indexes for the scheduler module.
/// Runs once when the application starts.
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
        _logger.LogInformation("Index Creation Background Service started.");

        try
        {
            await CreateIndexesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MongoDB indexes");
            throw;
        }
        finally
        {
            _logger.LogInformation("Index Creation Background Service finished.");
        }
    }

    /// <summary>
    /// Creates indexes for scheduler collections.
    /// </summary>
    private async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MongoDB index creation for scheduler module...");

        // Using scope to get dependencies with correct lifetime
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMongoContext>();

        await CreateJobTriggerIndexesAsync(context, cancellationToken);
        await CreateJobDefinitionIndexesAsync(context, cancellationToken);
        await CreateJobExecutionIndexesAsync(context, cancellationToken);

        _logger.LogInformation("MongoDB index creation for scheduler module completed.");
    }

    private async Task CreateJobDefinitionIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobDefinition>(Constants.JobDefinitionsCollectionName);
        
        // Unique index by job name
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(
                Builders<JobDefinition>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created unique index JobDefinition.Name");

        // Unique index by job type
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobDefinition>(
                Builders<JobDefinition>.IndexKeys.Ascending(x => x.JobType),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created unique index JobDefinition.JobType");
    }

    private async Task CreateJobTriggerIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobTrigger>(Constants.JobTriggersCollectionName);
        
        // Index for searching triggers by next run time
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(
                Builders<JobTrigger>.IndexKeys.Ascending(x => x.NextRunTime),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created index JobTrigger.NextRunTime");

        // Unique index by trigger identifier
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobTrigger>(
                Builders<JobTrigger>.IndexKeys.Ascending(x => x.TriggerId),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created unique index JobTrigger.TriggerId");
    }

    private async Task CreateJobExecutionIndexesAsync(IMongoContext context, CancellationToken cancellationToken)
    {
        var collection = context.GetCollection<JobExecution>(Constants.JobExecutionsCollectionName);
        
        // Unique index by trigger identifier (to ensure only one execution per trigger)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.TriggerId),
                new CreateIndexOptions { Unique = true, Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created unique index JobExecution.TriggerId");

        // Index for searching running jobs by lock
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.LockId).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created compound index JobExecution.LockId+Status");

        // Index by job type and status
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.JobType).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created compound index JobExecution.JobType+Status");

        // Index for searching executions by trigger and status (used in ScheduleJobAsync)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.TriggerId).Ascending(x => x.Status),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created compound index JobExecution.TriggerId+Status");

        // Index for searching expired locks (used in CreateAndStartJobExecution and TryCaptureExistingExecutionLock)
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<JobExecution>(
                Builders<JobExecution>.IndexKeys.Ascending(x => x.LockExpiry),
                new CreateIndexOptions { Background = true }),
            cancellationToken: cancellationToken);
        
        _logger.LogDebug("Created index JobExecution.LockExpiry");
    }
}