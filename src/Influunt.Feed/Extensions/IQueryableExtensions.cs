using System.Linq;
using Influunt.Feed.Entity;

namespace Influunt.Feed.Services;

public static class IQueryableExtensions
{
    public static IQueryable<FeedItem> GetChunkedFeed(this IQueryable<FeedItem> feed, int? offset, int count)
    {
        return offset == null ? feed : feed.Skip(offset.Value).Take(count);
    }
}