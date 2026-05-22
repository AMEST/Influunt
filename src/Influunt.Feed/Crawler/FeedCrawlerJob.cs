using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Influunt.Scheduler.Abstractions.Models;
using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Influunt.Feed.Crawler;

public class FeedCrawlerJob : IJob
{
    private readonly IUserService _userService;
    private readonly IScheduler _scheduler;
    private readonly ILogger<FeedCrawlerJob> _logger;
    private readonly CrawlerOptions _options;

    public FeedCrawlerJob(IOptions<CrawlerOptions> options,
        IUserService userService,
        IScheduler scheduler,
        ILogger<FeedCrawlerJob> logger)
    {
        _options = options.Value;
        _userService = userService;
        _scheduler = scheduler;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Feed Crawler job started");
        var minimumLastActivityDate = DateTime.UtcNow - TimeSpan.FromDays(_options.LastActivityDaysAgo);
        var users = await _userService.GetUsers();
        
        foreach (var user in users.Where(x => x.LastActivity > minimumLastActivityDate))
        {
            var triggerId = await _scheduler.ScheduleOneTimeJobAsync<UserFeedChannelUpdateJob, UserFeedChannelUpdateJobSettings>(new UserFeedChannelUpdateJobSettings()
            {
                User = user
            });
            _logger.LogInformation("Feed channel update job enqueued for user {email} with trigger id {triggerId}", user.Email, triggerId);
        }
        _logger.LogInformation("Feed Crawler job ended");
    }

}
