using Influunt.Feed.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Feed
{
    /// <summary>
    /// Сервис каналов
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// Получение канала по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FeedChannel> Get(string id);

        /// <summary>
        /// Добавление канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task<FeedChannel> Add(FeedChannel channel);

        /// <summary>
        /// Удаление канала
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task Remove(User user, FeedChannel channel);

        /// <summary>
        /// Обновление информации канала
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task Update(User user, FeedChannel channel);

        /// <summary>
        /// Получение списка каналов пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<FeedChannel>> GetUserChannels(User user);
    }
}