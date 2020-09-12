using Influunt.Feed.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace Influunt.Feed.Rss
{
    public class RssFeedService : IFeedService
    {
        private readonly IChannelService _channelService;
        private readonly IMemoryCache _memoryCache;
        private HttpClient _rssClient;

        public RssFeedService(IChannelService channelService, IMemoryCache memoryCache)
        {
            _channelService = channelService;
            _memoryCache = memoryCache;
            _rssClient = new HttpClient();
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10)
        {
            var userChannels = await _channelService.GetUserChannels(user);
            var feed = new List<FeedItem>();
            foreach (var channel in userChannels)
            {
                if (_memoryCache.TryGetValue($"channel_url_{channel.Url}", out List<FeedItem> channelFeed))
                {
                    feed.AddRange(channelFeed);
                    continue;
                }

                channelFeed = await GetFeedFromChannel(channel);
                _memoryCache.Set($"channel_url_{channel.Url}", channelFeed, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
                feed.AddRange(channelFeed);
            }

            feed = feed.OrderBy(f => string.IsNullOrWhiteSpace(f?.Date)
                ? DateTime.UtcNow
                : DateTime.Parse(f.Date)).ToList();

            if (offset != null)
            {
                var remainingElements = feed.Count - offset.Value;
                if (remainingElements < count)
                    return feed.GetRange(offset.Value <= feed.Count? offset.Value: feed.Count, remainingElements < 0 ? 0 : remainingElements);

                return feed.GetRange(offset.Value, count);
            }

            return feed;
        }

        public async Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null, int count = 10)
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
                    return feed.GetRange(offset.Value <= feed.Count ? offset.Value : feed.Count, remainingElements < 0 ? 0 : remainingElements);

                return feed.GetRange(offset.Value, count);
            }

            return feed;
        }

        private async Task<List<FeedItem>> GetFeedFromChannel(FeedChannel channel)
        {
            var result = await _rssClient.GetAsync(channel.Url);
            var xmlRss = await result.Content.ReadAsStreamAsync();


            XmlSerializer ser = new XmlSerializer(typeof(RssBody));
            RssBody rssBody;
            rssBody = (RssBody) ser.Deserialize(xmlRss);

            return rssBody.Channel.Item.Select(rssItem => new FeedItem
            {
                Title = rssItem.Title,
                Description = rssItem.Description,
                Date = rssItem.PubDate,
                Link = rssItem.Link?.ToString(),
                ChannelName = channel.Name
            }).ToList();
        }
    }
}