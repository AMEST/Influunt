using Influunt.Feed.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Feed;

/// <summary>
///     User feed service
/// </summary>
public interface IFeedService
{
    /// <summary>
    ///     Get user feed
    /// </summary>
    /// <param name="user"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10);

    /// <summary>
    ///     Get feed by user and channel
    /// </summary>
    /// <param name="user"></param>
    /// <param name="channel"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null, int count = 10);

    /// <summary>
    ///     Remove all feed items owned to this user and channel
    /// </summary>
    /// <param name="user"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task RemoveFeedByChannel(User user, FeedChannel channel);

    /// <summary>
    ///     Try add new posts in user feed
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newFeedItems"></param>
    /// <returns></returns>
    Task<int> TryAddToFeed(User user, IEnumerable<FeedItem> newFeedItems, FeedChannel channel = null);
}