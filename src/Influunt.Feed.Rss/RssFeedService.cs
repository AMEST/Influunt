using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Influunt.Feed.Entity;

namespace Influunt.Feed.Rss
{
    internal class RssFeedService : IFeedService, IDisposable
    {
        private readonly IChannelService _channelService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RssFeedService> _logger;
        private readonly HttpClient _rssClient;

        public RssFeedService(IChannelService channelService, IDistributedCache distributedCache, ILogger<RssFeedService> logger)
        {
            _channelService = channelService;
            _distributedCache = distributedCache;
            _logger = logger;
            _rssClient = new HttpClient();
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10)
        {
            var sw = Stopwatch.StartNew();
            var userChannels = await _channelService.GetUserChannels(user);
            var feed = new List<FeedItem>();

            var taskList = new List<Task<List<FeedItem>>>();

            foreach (var channel in userChannels.Where( c => !c.Hidden))
            {
                var channelTask = Task.Run(async () =>
                {
                    if (_distributedCache.TryGetValue($"channel_url_{channel.Url}", out List<FeedItem> channelFeed))
                        return channelFeed;

                    channelFeed = await GetFeedFromChannel(channel);
                    if (channelFeed.Any())
                        _distributedCache.Set($"channel_url_{channel.Url}", channelFeed, new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        });
                    return channelFeed;
                });
                taskList.Add(channelTask);
            }

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
            feed = feed.OrderBy(f => 
                string.IsNullOrWhiteSpace(f?.Date)
                ? DateTime.UtcNow
                : DateTime.Parse(f.Date)
                ).ToList();
            feed.Reverse();

            if (offset == null) return feed;

            var remainingElements = feed.Count - offset.Value;
            if (remainingElements < count)
                return feed.GetRange(offset.Value <= feed.Count ? offset.Value : feed.Count,
                    remainingElements < 0 ? 0 : remainingElements);
            
            return feed.GetRange(offset.Value, count);

        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null,
            int count = 10)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return new List<FeedItem>();

            if (!_distributedCache.TryGetValue($"channel_url_{channel.Url}", out List<FeedItem> feed))
            {
                feed = await GetFeedFromChannel(channel);
                _distributedCache.Set($"channel_url_{channel.Url}", feed, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            if (offset == null) return feed;
            var remainingElements = feed.Count - offset.Value;
            if (remainingElements < count)
                return feed.GetRange(offset.Value <= feed.Count ? offset.Value : feed.Count,
                    remainingElements < 0 ? 0 : remainingElements);

            return feed.GetRange(offset.Value, count);

        }

        private async Task<List<FeedItem>> GetFeedFromChannel(FeedChannel channel)
        {
            try
            {
                using (var result = await _rssClient.GetAsync(channel.Url))
                using (var xmlRss = await result.Content.ReadAsStreamAsync())
                {
                    var ser = new XmlSerializer(typeof(RssBody));
                    var rssBody = (RssBody)ser.Deserialize(xmlRss);

                    return rssBody.Channel.Item.Select(rssItem => new FeedItem
                    {
                        Title = rssItem.Title,
                        Description = rssItem.Description,
                        Date = rssItem.PubDate,
                        Link = rssItem.Link?.ToString(),
                        ChannelName = channel.Name ?? ""
                    }.NormalizeDescription()).ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Can not get rss feed from \nchannel: {channelName}\nurl: {channelUrl}\n with error: {message} ",
                    channel.Name, channel.Url, e.Message);
                return new List<FeedItem>();
            }
        }

        public void Dispose()
        {
            _rssClient?.Dispose();
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}