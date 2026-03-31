using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;

namespace Influunt.Scheduler;

public static class SchedulerModule
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, Action<JobSchedulerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        // Register IMongoContext
        services.AddSingleton<IMongoContext, MongoContext>();
        
        // Register JobScheduler
        services.AddSingleton<IScheduler, JobScheduler>();
        
        services.AddHostedService<IndexCreationBackgroundService>();
        
        // Register BSON class maps
        BsonClassMap.RegisterClassMap(new JobDefinitionClassMap());
        BsonClassMap.RegisterClassMap(new JobExecutionClassMap());
        BsonClassMap.RegisterClassMap(new JobTriggerClassMap());
        
        return services;
    }

    public static IServiceCollection AddJobSchedulerHost(this IServiceCollection services, Action<JobSchedulerOptions> configureOptions)
    {
        services.AddJobScheduler(configureOptions);
        services.AddSingleton<IHostedService>(s => (JobScheduler)s.GetRequiredService<IScheduler>());
        return services;
    }
}
