using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.Mongo.Abstractions;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace Influunt.Storage.Services
{
    public class UserService: IUserService
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IMongoRepository<User> userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userRepository.Get();
        }

        public async Task<User> GetCurrentUser()
        {
            var contextUser = User.FromIdentity(_httpContextAccessor.HttpContext.User);
            if (contextUser == null)
                return null;

            var user = await GetUserByEmail(contextUser.Email);

            if (user == null) return await Add(contextUser);

            user.LastActivity = DateTime.UtcNow;
            await Update(user);
            return user;

        }

        public Task<User> GetUserById(string id)
        {
            return _userRepository.Get(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = (await GetUsers()).SingleOrDefault(u => u?.Email?.ToLower() == email?.ToLower());
            return user;
        }

        public async Task<User> Add(User user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            user.LastActivity = DateTime.UtcNow;
            await _userRepository.Create(user);
            return user;
        }

        public Task Update(User updatedUser)
        {
            _userRepository.Update(updatedUser);
            return Task.CompletedTask;
        }

        public Task Remove(string id)
        {
            _userRepository.Delete(id);
            return Task.CompletedTask;
        }
    }
}