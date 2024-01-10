using Influunt.Feed.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Feed;

/// <summary>
/// Channel service
/// </summary>
public interface IChannelService
{
    /// <summary>
    /// Get channel by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<FeedChannel> Get(string id);

    /// <summary>
    /// Add chammel
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task<FeedChannel> Add(FeedChannel channel);

    /// <summary>
    /// Remove channel
    /// </summary>
    /// <param name="user"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task Remove(User user, FeedChannel channel);

    /// <summary>
    /// Update channel
    /// </summary>
    /// <param name="user"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task Update(User user, FeedChannel channel);

    /// <summary>
    /// Get user channels
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEnumerable<FeedChannel>> GetUserChannels(User user);
}