using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Influunt.Scheduler;

public static class SchedulerModule
{
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, Action<JobSchedulerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IScheduler, JobScheduler>();
        return services;
    }

    public static IServiceCollection AddJobSchedulerHost(this IServiceCollection services, Action<JobSchedulerOptions> configureOptions)
    {
        services.AddJobScheduler(configureOptions);
        services.AddSingleton<IHostedService>(s => (JobScheduler)s.GetRequiredService<IScheduler>());
        BsonClassMap.RegisterClassMap(new JobDefinitionClassMap());
        return services;
    }
}