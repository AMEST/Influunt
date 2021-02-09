using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed
{
    /// <summary>
    /// Сервис избранных постов
    /// </summary>
    public interface IFavoriteFeedService
    {
        /// <summary>
        /// Добавление поста в избранное
        /// </summary>
        /// <param name="user"></param>
        /// <param name="favorite"></param>
        /// <returns></returns>
        Task<FavoriteFeedItem> Add(User user, FeedItem favorite);

        /// <summary>
        /// Удаление поста из избранного
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Remove(User user, string id);

        /// <summary>
        /// Получение избранных постов пользователя
        /// </summary>
        Task<IEnumerable<FavoriteFeedItem>> GetUserFavorites(User user, int? offset);
    }
}