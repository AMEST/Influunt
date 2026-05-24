using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Influunt.Scheduler.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Skidbladnir.Utility.Common;

namespace Influunt.Feed.Crawler;

public class UserFeedChannelUpdateJob : IJob<UserFeedChannelUpdateJobSettings>
{
    private readonly IEnumerable<IFeedSourceProvider> _feedSourceProviders;
    private readonly IFeedService _feedService;
    private readonly IChannelService _channelService;
    private readonly ILogger<UserFeedChannelUpdateJob> _logger;

    public UserFeedChannelUpdateJob(IEnumerable<IFeedSourceProvider> feedSourceProviders,
        IFeedService feedService, 
        IChannelService channelService,
        ILogger<UserFeedChannelUpdateJob> logger)
    {
        _feedSourceProviders = feedSourceProviders;
        _feedService = feedService;
        _channelService = channelService;
        _logger = logger;
    }

    public async Task ExecuteAsync(UserFeedChannelUpdateJobSettings settings, CancellationToken cancellationToken)
    {
        if (settings?.User is null)
        {
            _logger.LogError("User is null");
            return;
        }
        _logger.BeginScope("Update feed channels job for user {email}", settings.User.Email);
        var channels = await Try.DoAsync(() => _channelService.GetUserChannels(settings.User));
        if (channels is null)
            return;

        try
        {
            var fetchTasks = channels.Select(x => FetchFeedFromChannel(x, settings.User));
            await Task.WhenAll(fetchTasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Can't fetch feed");
        }
    }


    private async Task FetchFeedFromChannel(FeedChannel channel, User user)
    {
        var remoteFeedProvider = _feedSourceProviders.FirstOrDefault(x => x.CanProcessChannel(channel));
        if (remoteFeedProvider is null)
            return;
        var remoteFeed = await remoteFeedProvider.GetRemoteFeed(channel);
        var count = await _feedService.TryAddToFeed(user, remoteFeed, channel);
        _logger.LogInformation("Added {count} posts from channel {channelId} to {email} feed", count, channel.Id, user.Email);
    }

}
