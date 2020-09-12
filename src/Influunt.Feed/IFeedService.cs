using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

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
        /// <returns></returns>
        Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10);

        /// <summary>
        /// Получение ленты пользователя по каналу
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null, int count = 10);
    }
}