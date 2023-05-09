using Influunt.Feed;
using Influunt.Host.ViewModels;
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

        /// <inheritdoc />
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
        public async Task<IEnumerable<FeedItemViewModel>> Get([FromQuery] int? offset)
        {
            var user = await _userService.GetCurrentUser();
            var feed = await _feedService.GetFeed(user, offset);
            var userChannels = await _channelService.GetUserChannels(user);
            return feed.ToModel(userChannels);
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
        public async Task<IEnumerable<FeedItemViewModel>> Get(string id, [FromQuery] int? offset)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            var feed = await _feedService.GetFeed(user, channel, offset);
            return feed.ToModel(new[] { channel });
        }
    }
}
