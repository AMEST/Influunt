using Influunt.Feed;
using Influunt.Feed.Entity;
using Microsoft.AspNetCore.Http;
using Skidbladnir.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Storage.Services;

internal class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IRepository<User> userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _userRepository.GetAll().ToArrayAsync();
    }

    public async Task<User> GetCurrentUser()
    {
        var contextUser = User.FromIdentity(_httpContextAccessor.HttpContext.User);
        if (contextUser is null)
            return null;

        var user = await GetUserByEmail(contextUser.Email);

        if (user is null) return await Add(contextUser);

        user.LastActivity = DateTime.UtcNow;
        await Update(user);
        return user;
    }

    public Task<User> GetUserById(string id)
    {
        return _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return user;
    }

    public async Task<User> Add(User user)
    {
        user.LastActivity = DateTime.UtcNow;
        await _userRepository.Create(user);
        return user;
    }

    public Task Update(User updatedUser)
    {
        return _userRepository.Update(updatedUser);
    }

    public Task Remove(User user)
    {
        return _userRepository.Delete(user);
    }
}