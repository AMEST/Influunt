using Influunt.Feed.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Feed
{
    /// <summary>
    /// Сервис работы с леной
    /// </summary>
    public interface IFeedService
    {
        /// <summary>
        /// Получение ленты пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10);

        /// <summary>
        /// Получение ленты пользователя по каналу
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null, int count = 10);
    }
}