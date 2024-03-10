using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed;

public interface IFeedSourceProvider
{

    /// <summary>
    ///  Get remote feed
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task<IEnumerable<FeedItem>> GetRemoteFeed(FeedChannel channel);

    /// <summary>
    /// Can this provider process this channel
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    bool CanProcessChannel(FeedChannel channel);
}