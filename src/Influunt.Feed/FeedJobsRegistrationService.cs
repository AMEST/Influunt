using System.Threading;
using System.Threading.Tasks;
using Influunt.Feed.Crawler;
using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.Hosting;

namespace Influunt.Feed;

public class FeedJobsRegistrationService : BackgroundService
{
    private const string CrawlerJobTriggerId = "RegularFeedCrawlerJob";
    private readonly IScheduler _scheduler;

    public FeedJobsRegistrationService(IScheduler scheduler)
    {
        _scheduler = scheduler;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _scheduler.RegisterJobAsync<FeedCrawlerJob>("Users feeds updating job", "The task starts updating RSS feeds for each active user");
        await _scheduler.RegisterJobAsync<UserFeedChannelUpdateJob, UserFeedChannelUpdateJobSettings>("User feed updating job", "The task receives new posts from the user's RSS feeds and adds them to the news feed");
        
        await _scheduler.UnScheduleJobAsync(CrawlerJobTriggerId);
        await _scheduler.ScheduleRecurringJobAsync<FeedCrawlerJob>("*/30 * * * *", CrawlerJobTriggerId);
    }

}
