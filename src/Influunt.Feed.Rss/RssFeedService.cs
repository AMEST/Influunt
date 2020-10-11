using Influunt.Feed.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Influunt.Feed.Rss
{
    public class RssFeedService : IFeedService
    {
        private readonly IChannelService _channelService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RssFeedService> _logger;
        private HttpClient _rssClient;

        public RssFeedService(IChannelService channelService, IMemoryCache memoryCache, ILogger<RssFeedService> logger)
        {
            _channelService = channelService;
            _memoryCache = memoryCache;
            _logger = logger;
            _rssClient = new HttpClient();
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10)
        {
            var sw = Stopwatch.StartNew();
            var userChannels = await _channelService.GetUserChannels(user);
            var feed = new List<FeedItem>();

            var taskList = new List<Task<List<FeedItem>>>();

            foreach (var channel in userChannels)
            {
                var channelTask = Task.Run<List<FeedItem>>(async () =>
                {
                    if (_memoryCache.TryGetValue($"channel_url_{channel.Url}", out List<FeedItem> channelFeed))
                    {
                        return channelFeed;
                    }

                    channelFeed = await GetFeedFromChannel(channel);
                    _memoryCache.Set($"channel_url_{channel.Url}", channelFeed, new MemoryCacheEntryOptions
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
                feed.AddRange(task.Result);
            }
            _logger.LogDebug($"Elapsed time for getting user ({user.Id}) feed: {sw.Elapsed.TotalMilliseconds}ms");
            feed = feed.OrderBy(f => string.IsNullOrWhiteSpace(f?.Date)
                ? DateTime.UtcNow
                : DateTime.Parse(f.Date)).ToList();

            if (offset != null)
            {
                var remainingElements = feed.Count - offset.Value;
                if (remainingElements < count)
                    return feed.GetRange(offset.Value <= feed.Count ? offset.Value : feed.Count,
                        remainingElements < 0 ? 0 : remainingElements);

                return feed.GetRange(offset.Value, count);
            }

            return feed;
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null,
            int count = 10)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return new List<FeedItem>();

            if (!_memoryCache.TryGetValue($"channel_url_{channel.Url}", out List<FeedItem> feed))
            {
                feed = await GetFeedFromChannel(channel);
                _memoryCache.Set($"channel_url_{channel.Url}", feed, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            if (offset != null)
            {
                var remainingElements = feed.Count - offset.Value;
                if (remainingElements < count)
                    return feed.GetRange(offset.Value <= feed.Count ? offset.Value : feed.Count,
                        remainingElements < 0 ? 0 : remainingElements);

                return feed.GetRange(offset.Value, count);
            }

            return feed;
        }

        private async Task<List<FeedItem>> GetFeedFromChannel(FeedChannel channel)
        {
            try
            {
                var result = await _rssClient.GetAsync(channel.Url);

                var xmlRss = await result.Content.ReadAsStreamAsync();


                XmlSerializer ser = new XmlSerializer(typeof(RssBody));
                RssBody rssBody;
                rssBody = (RssBody)ser.Deserialize(xmlRss);

                return rssBody.Channel.Item.Select(rssItem => new FeedItem
                {
                    Title = rssItem.Title,
                    Description = rssItem.Description,
                    Date = rssItem.PubDate,
                    Link = rssItem.Link?.ToString(),
                    ChannelName = channel.Name
                }).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Can not get rss feed from \nchannel: {channelName}\nurl: {channelUrl}\n with error: {message} ",
                    channel.Name, channel.Url, e.Message);
                return new List<FeedItem>();
            }
        }
    }
}