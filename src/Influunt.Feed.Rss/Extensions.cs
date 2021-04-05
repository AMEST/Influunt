using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Influunt.Feed.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Influunt.Feed.Rss
{
    public static class Extensions
    {
        public static bool TryGetValue<TResult>(this IDistributedCache cache, string key, out TResult result)
        {
            var cacheValue = cache.Get(key);
            if (cacheValue == null)
            {
                result = default;
                return false;
            }

            result = JsonConvert.DeserializeObject<TResult>(Encoding.UTF8.GetString(cacheValue));
            cacheValue = null;
            return true;
        }

        public static void Set(this IDistributedCache cache, string key, object entry,
            DistributedCacheEntryOptions options)
        {
            var serializedEntry = JsonConvert.SerializeObject(entry);
            cache.Set(key, Encoding.UTF8.GetBytes(serializedEntry), options);
            serializedEntry = null;
        }


        public static IEnumerable<FeedItem> GetChunckedFeed(this IEnumerable<FeedItem> feed, int? offset, int count)
        {
            return offset == null ? feed : feed.Skip(offset.Value).Take(count);
        }

        public static bool IsAtomRss(this string xml)
        {
            return xml.Contains("xmlns=\"http://www.w3.org/2005/Atom\"");
        }

        public static List<FeedItem> FeedFromRss(this string xml, FeedChannel channel)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? "")))
            {
                var ser = new XmlSerializer(typeof(RssBody));
                var rssBody = (RssBody) ser.Deserialize(memoryStream);
                return rssBody.Channel.Item.Select(rssItem =>
                {
                    var item = new FeedItem
                    {
                        Title = rssItem.Title,
                        Description = rssItem.Description,
                        PubDate = DateTime.UtcNow.AddDays(-31),
                        Link = rssItem.Link?.ToString(),
                        ChannelName = channel.Name ?? ""
                    };

                    if (DateTime.TryParse(rssItem.PubDate, out var pubDate))
                        item.PubDate = pubDate;

                    return item.NormalizeDescription();
                }).ToList();
            }
        }

        public static List<FeedItem> FeedFromAtomRss(this string xml, FeedChannel channel)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? "")))
            {
                var ser = new XmlSerializer(typeof(Feed));
                var rssBody = (Feed) ser.Deserialize(memoryStream);
                return rssBody.Entry.Select(rssItem =>
                {
                    var item = new FeedItem
                    {
                        Title = rssItem.Title,
                        Description = string.IsNullOrEmpty(rssItem.Content?.Text)
                            ? rssItem.Summary
                            : rssItem.Content.Text,
                        PubDate = DateTime.UtcNow.AddDays(-31),
                        Link = rssItem.Link?.ToString(),
                        ChannelName = channel.Name ?? ""
                    };

                    if (DateTime.TryParse(rssItem.Published, out var pubDate))
                        item.PubDate = pubDate;

                    return item.NormalizeDescription();
                }).ToList();
            }
        }
    }
}