using Influunt.Scheduler.Abstractions.Models;

namespace Influunt.Scheduler.Abstractions.Services;
public interface IScheduler
{
    Task RegisterJobAsync<TJob, TSettings>(string name, string? description = null, bool allowConcurrentExecution = false) where TJob : IJob<TSettings>;
    Task RegisterJobAsync<TJob>(string name, string? description = null, bool allowConcurrentExecution = false) where TJob : IJob;
    Task<string> ScheduleOneTimeJobAsync<TJob>(DateTime? runAt = null, string? triggerId = null) where TJob : IJob;
    Task<string> ScheduleRecurringJobAsync<TJob>(string cronExpression, string? triggerId = null) where TJob : IJob;
    Task<string> ScheduleOneTimeJobAsync<TJob, TSettings>(TSettings settings, DateTime? runAt = null, string? triggerId = null) where TJob : IJob<TSettings>;
    Task<string> ScheduleRecurringJobAsync<TJob, TSettings>(string cronExpression, TSettings settings, string? triggerId = null) where TJob : IJob<TSettings>;
    Task UnScheduleJobAsync(string triggerId);
    Task<IEnumerable<JobDefinition>> GetRegisteredJobsAsync();
    Task<IEnumerable<JobTrigger>> GetScheduledTriggersAsync();
}