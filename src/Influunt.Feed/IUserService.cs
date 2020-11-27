using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Feed
{
    /// <summary>
    /// Сервис работы с пользователями
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsers();

        /// <summary>
        /// Получение текущего пользователя
        /// </summary>
        /// <returns></returns>
        Task<User> GetCurrentUser();

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User> GetUserById(string id);

        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetUserByEmail(string email);

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<User> Add(User user);

        /// <summary>
        /// Обновление информации пользователя
        /// </summary>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        Task Update(User updatedUser);

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Remove(string id);
    }
}