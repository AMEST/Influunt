using Influunt.Feed;
using Influunt.Feed.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFavoriteFeedService _favoriteFeedService;

        public FavoriteController(IUserService userService, IFavoriteFeedService favoriteFeedService)
        {
            _userService = userService;
            _favoriteFeedService = favoriteFeedService;
        }

        // GET: api/Favorite
        [HttpGet]
        public async Task<IEnumerable<FavoriteFeedItem>> Get()
        {
            var user = await _userService.GetCurrentUser();
            return await _favoriteFeedService.GetUserFavorites(user);
        }

        // POST: api/Favorite
        [HttpPost]
        public async Task Post([FromBody] FeedItem feedItem)
        {
            var user = await _userService.GetCurrentUser();
            await _favoriteFeedService.Add(user, feedItem);
        }

        // DELETE: api/Favorite/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userService.GetCurrentUser();
            await _favoriteFeedService.Remove(user, id);
        }
    }
}
