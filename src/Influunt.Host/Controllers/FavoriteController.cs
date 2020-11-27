using Influunt.Feed;
using Influunt.Feed.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Host.Controllers
{
    /// <summary>
    /// Favorites api
    /// </summary>
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

        /// <summary>
        /// Get Favorites
        /// </summary>
        /// <response code="200">Favorites</response>
        /// <response code="401">Unauthorize</response>
        [HttpGet]
        public async Task<IEnumerable<FavoriteFeedItem>> Get()
        {
            var user = await _userService.GetCurrentUser();
            return await _favoriteFeedService.GetUserFavorites(user);
        }

        // POST: api/Favorite
        /// <summary>
        /// Add feed to favorites
        /// </summary>
        /// <param name="feedItem"></param>
        /// <response code="401">Unauthorize</response>
        [HttpPost]
        public async Task Post([FromBody] FeedItem feedItem)
        {
            var user = await _userService.GetCurrentUser();
            await _favoriteFeedService.Add(user, feedItem);
        }

        // DELETE: api/Favorite/5
        /// <summary>
        /// Delete favorite by id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="401">Unauthorize</response>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userService.GetCurrentUser();
            await _favoriteFeedService.Remove(user, id);
        }
    }
}
