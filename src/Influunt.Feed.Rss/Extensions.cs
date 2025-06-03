using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Influunt.Feed.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Influunt.Feed.Rss;

public static class Extensions
{
    public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var cacheValue = await cache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(cacheValue))
            return default;
        return JsonConvert.DeserializeObject<T>(cacheValue);
    }

    public static Task SetAsync(this IDistributedCache cache, string key, object entry,
        DistributedCacheEntryOptions options)
    {
        var serializedEntry = JsonConvert.SerializeObject(entry);
        return cache.SetAsync(key, Encoding.UTF8.GetBytes(serializedEntry), options);
    }

    public static List<T> AsList<T>(this IEnumerable<T> source)
        => (source == null || source is List<T>) ? (List<T>)source : source.ToList();

    public static bool IsAtomRss(this string xml)
    {
        return xml.Contains("xmlns=\"http://www.w3.org/2005/Atom\"");
    }

    public static List<FeedItem> FeedFromRss(this string xml)
    {
        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? "")))
        {
            var ser = new XmlSerializer(typeof(RssBody));
            var rssBody = (RssBody)ser.Deserialize(memoryStream);
            return rssBody.Channel.Item.Select(rssItem =>
            {
                var item = new FeedItem
                {
                    Title = rssItem.Title,
                    Description = rssItem.Description,
                    PubDate = DateTime.UtcNow,
                    Link = rssItem.Link?.ToString(),
                };

                if (DateTime.TryParse(rssItem.PubDate, out var pubDate))
                    item.PubDate = pubDate;

                return item.NormalizeDescription();
            }).ToList();
        }
    }

    public static List<FeedItem> FeedFromAtomRss(this string xml)
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? ""));
        var ser = new XmlSerializer(typeof(Feed));
        var rssBody = (Feed)ser.Deserialize(memoryStream);
        return rssBody.Entry.Select(rssItem =>
        {
            var item = new FeedItem
            {
                Title = rssItem.Title,
                Description = string.IsNullOrEmpty(rssItem.Content?.Text)
                    ? rssItem.Summary
                    : rssItem.Content.Text,
                PubDate = DateTime.UtcNow,
                Link = rssItem.Link?.ToString(),
            };

            if (DateTime.TryParse(rssItem.Published, out var pubDate))
                item.PubDate = pubDate;

            return item.NormalizeDescription();
        }).ToList();
    }
}