using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed
{
    /// <summary>
    ///     Favorite feed service
    /// </summary>
    public interface IFavoriteFeedService
    {
        /// <summary>
        /// Add item to favorites
        /// </summary>
        /// <param name="user"></param>
        /// <param name="favorite"></param>
        /// <returns></returns>
        Task<FavoriteFeedItem> Add(User user, FeedItem favorite);

        /// <summary>
        /// Remove from favorites
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Remove(User user, string id);

        /// <summary>
        /// Get all favorites from user
        /// </summary>
        Task<IEnumerable<FavoriteFeedItem>> GetUserFavorites(User user, int? offset);
    }
}