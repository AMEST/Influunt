using Influunt.Feed;
using Influunt.Feed.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.Host.Controllers
{
    /// <summary>
    /// Feed api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFeedService _feedService;
        private readonly IChannelService _channelService;

        public FeedController(IUserService userService, IFeedService feedService, IChannelService channelService)
        {
            _userService = userService;
            _feedService = feedService;
            _channelService = channelService;
        }

        /// <summary>
        /// Get Feed from all channels
        /// </summary>
        /// <param name="offset">feed offset</param>
        /// <response code="200">Feed</response>
        /// <response code="401">Unauthorize</response>
        [HttpGet]
        public async Task<IEnumerable<FeedItem>> Get([FromQuery] int? offset)
        {
            var user = await _userService.GetCurrentUser();
            return await _feedService.GetFeed(user, offset);
        }

        // GET: api/Feed/5
        /// <summary>
        /// Get feed from channel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset">feed offset</param>
        /// <response code="200">Feed</response>
        /// <response code="401">Unauthorize</response>
        [HttpGet("{id}")]
        public async Task<IEnumerable<FeedItem>> Get(string id, [FromQuery] int? offset)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            return await _feedService.GetFeed(user, channel, offset);
        }
    }
}
