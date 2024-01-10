using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skidbladnir.Utility.Common;

namespace Influunt.Feed.Crawler;

public class FeedCrawlerBackgroundWorker : BackgroundService
{
    private readonly IEnumerable<IFeedSourceProvider> _feedSourceProviders;
    private readonly IFeedService _feedService;
    private readonly IUserService _userService;
    private readonly IChannelService _channelService;
    private readonly CrawlerOptions _options;
    private readonly ILogger<FeedCrawlerBackgroundWorker> _logger;

    public FeedCrawlerBackgroundWorker(IEnumerable<IFeedSourceProvider> feedSourceProviders,
        IFeedService feedService, IUserService userService,
        IOptions<CrawlerOptions> options,
        IChannelService channelService,
        ILogger<FeedCrawlerBackgroundWorker> logger)
    {
        _feedSourceProviders = feedSourceProviders;
        _feedService = feedService;
        _userService = userService;
        _channelService = channelService;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Feed Crawler Background Worker running.");


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await InnerExecute(stoppingToken);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                    break;
                _logger.LogError(e, "Error in Feed Crawler Background Worker when fetch feeds.");
            }
            await Task.Delay(_options.FetchInterval);
        }
        _logger.LogInformation("Feed Crawler Background Worker is stopping.");

    }

    private async Task InnerExecute(CancellationToken token)
    {
        var minimumLastActivityDate = DateTime.UtcNow - TimeSpan.FromDays(_options.LastActivityDaysAgo);
        var users = await _userService.GetUsers();
        token.ThrowIfCancellationRequested();
        foreach (var user in users.Where(x => x.LastActivity > minimumLastActivityDate))
        {
            token.ThrowIfCancellationRequested();
            var channels = await Try.DoAsync(() => _channelService.GetUserChannels(user));
            if (channels == null)
                continue;
            try
            {
                var fetchTasks = channels.Select(x => FetchFeedFromChannel(x, user, token));
                await Task.WhenAll(fetchTasks);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                    throw;
                _logger.LogError(e, $"Can't fetch feed for user {user.Id}");
            }
        }
    }

    private async Task FetchFeedFromChannel(FeedChannel channel, User user, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var remoteFeedProvider = _feedSourceProviders.FirstOrDefault(x => x.CanProcessChannel(channel));
        if (remoteFeedProvider == null)
            return;
        var remoteFeed = await remoteFeedProvider.GetRemoteFeed(channel);
        var count = await _feedService.TryAddToFeed(user, remoteFeed, channel);
        _logger.LogInformation($"For user {user.Id} added {count} posts from channel {channel.Id}");
    }
}