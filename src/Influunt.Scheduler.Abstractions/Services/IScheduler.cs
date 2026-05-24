using Influunt.Scheduler.Abstractions.Models;

namespace Influunt.Scheduler.Abstractions.Services;

/// <summary>
/// Scheduler service that manages job registration and scheduling.
/// </summary>
public interface IScheduler
{
    /// <summary>
    /// Registers a job with the specified name and optional description.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob{TSettings}"/>.</typeparam>
    /// <typeparam name="TSettings">The type of the job settings.</typeparam>
    /// <param name="name">The name of the job.</param>
    /// <param name="description">An optional description of the job.</param>
    /// <param name="allowConcurrentExecution">Whether the job allows concurrent execution.</param>
    Task RegisterJobAsync<TJob, TSettings>(string name, string? description = null, bool allowConcurrentExecution = false) where TJob : IJob<TSettings>;

    /// <summary>
    /// Registers a job with the specified name and optional description.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob"/>.</typeparam>
    /// <param name="name">The name of the job.</param>
    /// <param name="description">An optional description of the job.</param>
    /// <param name="allowConcurrentExecution">Whether the job allows concurrent execution.</param>
    Task RegisterJobAsync<TJob>(string name, string? description = null, bool allowConcurrentExecution = false) where TJob : IJob;

    /// <summary>
    /// Schedules a one-time job to run at the specified time.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob"/>.</typeparam>
    /// <param name="runAt">The time at which the job should run. If null, the job will run as soon as possible.</param>
    /// <param name="triggerId">An optional identifier for the trigger.</param>
    /// <returns>The identifier of the scheduled trigger.</returns>
    Task<string> ScheduleOneTimeJobAsync<TJob>(DateTime? runAt = null, string? triggerId = null) where TJob : IJob;

    /// <summary>
    /// Schedules a recurring job based on the specified cron expression.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob"/>.</typeparam>
    /// <param name="cronExpression">The cron expression defining the schedule.</param>
    /// <param name="triggerId">An optional identifier for the trigger.</param>
    /// <returns>The identifier of the scheduled trigger.</returns>
    Task<string> ScheduleRecurringJobAsync<TJob>(string cronExpression, string? triggerId = null) where TJob : IJob;

    /// <summary>
    /// Schedules a one-time job with settings to run at the specified time.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob{TSettings}"/>.</typeparam>
    /// <typeparam name="TSettings">The type of the job settings.</typeparam>
    /// <param name="settings">The settings for the job.</param>
    /// <param name="runAt">The time at which the job should run. If null, the job will run as soon as possible.</param>
    /// <param name="triggerId">An optional identifier for the trigger.</param>
    /// <returns>The identifier of the scheduled trigger.</returns>
    Task<string> ScheduleOneTimeJobAsync<TJob, TSettings>(TSettings settings, DateTime? runAt = null, string? triggerId = null) where TJob : IJob<TSettings>;

    /// <summary>
    /// Schedules a recurring job with settings based on the specified cron expression.
    /// </summary>
    /// <typeparam name="TJob">The type of the job implementing <see cref="IJob{TSettings}"/>.</typeparam>
    /// <typeparam name="TSettings">The type of the job settings.</typeparam>
    /// <param name="cronExpression">The cron expression defining the schedule.</param>
    /// <param name="settings">The settings for the job.</param>
    /// <param name="triggerId">An optional identifier for the trigger.</param>
    /// <returns>The identifier of the scheduled trigger.</returns>
    Task<string> ScheduleRecurringJobAsync<TJob, TSettings>(string cronExpression, TSettings settings, string? triggerId = null) where TJob : IJob<TSettings>;

    /// <summary>
    /// Unschedules a job with the specified trigger identifier.
    /// </summary>
    /// <param name="triggerId">The identifier of the trigger to unschedule.</param>
    Task UnScheduleJobAsync(string triggerId);

    /// <summary>
    /// Retrieves all registered job definitions.
    /// </summary>
    /// <returns>A collection of registered job definitions.</returns>
    Task<IEnumerable<JobDefinition>> GetRegisteredJobsAsync();

    /// <summary>
    /// Retrieves all scheduled job triggers.
    /// </summary>
    /// <returns>A collection of scheduled job triggers.</returns>
    Task<IEnumerable<JobTrigger>> GetScheduledTriggersAsync();
}
