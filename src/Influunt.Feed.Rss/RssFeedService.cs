using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed.Rss
{
    internal class RssFeedService : IFeedService, IDisposable
    {
        private readonly IChannelService _channelService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RssFeedService> _logger;
        private readonly RssClient _rssClient;

        public RssFeedService(RssClient rssClient, IChannelService channelService, IDistributedCache distributedCache,
            ILogger<RssFeedService> logger)
        {
            _rssClient = rssClient;
            _channelService = channelService;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10)
        {
            var sw = Stopwatch.StartNew();
            var userChannels = await _channelService.GetUserChannels(user);
            var feed = new List<FeedItem>();

            var taskList = userChannels
                .Where(c => !c.Hidden)
                .Select(GetFeedFromChannelCached)
                .ToList();

            await Task.WhenAll(taskList);

            foreach (var task in taskList)
            {
                var result = task.Result;
                feed.AddRange(result);
                result.Clear();
                task.Dispose();
            }

            taskList.Clear();

            _logger.LogDebug($"Elapsed time for getting user ({user.Id}) feed: {sw.Elapsed.TotalMilliseconds}ms");
            feed = feed.OrderByDescending(f => f.PubDate).ToList();

            return feed.GetChunkedFeed(offset, count);
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null,
            int count = 10)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return new List<FeedItem>();

            var feed = await _distributedCache.GetAsync<List<FeedItem>>($"channel_url_{channel.Url}");

            if (feed != null && feed.Count != 0)
                return feed.GetChunkedFeed(offset, count);

            feed = await _rssClient.GetFeed(channel);
            await _distributedCache.SetAsync($"channel_url_{channel.Url}", feed, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return feed.GetChunkedFeed(offset, count);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        private async Task<List<FeedItem>> GetFeedFromChannelCached(FeedChannel channel)
        {
            var channelFeed = await _distributedCache.GetAsync<List<FeedItem>>($"channel_url_{channel.Url}");
            if (channelFeed != null && channelFeed.Count != 0)
                return channelFeed;

            channelFeed = await _rssClient.GetFeed(channel);
            if (channelFeed.Any())
                await _distributedCache.SetAsync($"channel_url_{channel.Url}", channelFeed, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            return channelFeed;
        }
    }
}