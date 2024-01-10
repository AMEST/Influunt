using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed.Rss;

internal class RssFeedSourceProvider : IFeedSourceProvider, IDisposable
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RssFeedSourceProvider> _logger;
    private readonly RssClient _rssClient;

    public RssFeedSourceProvider(RssClient rssClient, IDistributedCache distributedCache,
        ILogger<RssFeedSourceProvider> logger)
    {
        _rssClient = rssClient;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<IEnumerable<FeedItem>> GetRemoteFeed(FeedChannel channel)
    {
        var sw = Stopwatch.StartNew();
        var remoteFeed = await GetFeedFromChannelCached(channel);
        _logger.LogDebug($"Elapsed time for getting channel ({channel.Id}) feed: {sw.Elapsed.TotalMilliseconds}ms");
        return remoteFeed;
    }

    public bool CanProcessChannel(FeedChannel channel) => true;

    public void Dispose()
    {
        GC.Collect();
        GC.SuppressFinalize(this);
    }

    private async Task<List<FeedItem>> GetFeedFromChannelCached(FeedChannel channel)
    {
        var channelFeed = await _distributedCache.GetAsync<List<FeedItem>>($"channel_url_{channel.Url}");
        if (channelFeed is not null && channelFeed.Count != 0)
            return channelFeed;

        channelFeed = (await _rssClient.GetFeed(channel)).AsList();
        if (channelFeed.Count != 0)
            await _distributedCache.SetAsync($"channel_url_{channel.Url}", channelFeed, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        return channelFeed;
    }
}