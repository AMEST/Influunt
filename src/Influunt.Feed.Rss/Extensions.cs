using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}