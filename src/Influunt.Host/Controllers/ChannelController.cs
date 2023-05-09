using System;
using Influunt.Feed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Influunt.Host.ViewModels;

namespace Influunt.Host.Controllers
{
    /// <summary>
    /// Channels api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IChannelService _channelService;
        private readonly IFeedService _feedService;

        /// <inheritdoc />
        public ChannelController(IUserService userService,
            IChannelService channelService,
            IFeedService feedService)
        {
            _userService = userService;
            _channelService = channelService;
            _feedService = feedService;
        }

        /// <summary>
        /// All user channels
        /// </summary>
        /// <response code="200">Channels array</response>
        /// <response code="401">Unauthorize</response>
        [HttpGet]
        public async Task<IEnumerable<FeedChannelViewModel>> Get()
        {
            var user = await _userService.GetCurrentUser();
            return (await _channelService.GetUserChannels(user)).ToModel();
        }


        /// <summary>
        /// Get user channel by id
        /// </summary>
        /// <param name="id">Channel id</param>
        /// <response code="200">Channel</response>
        /// <response code="401">Unauthorize</response>
        [HttpGet("{id}")]
        public async Task<FeedChannelViewModel> Get(string id)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            if (channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return channel.ToModel();

            Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return null;
        }

        /// <summary>
        /// Add channel
        /// </summary>
        /// <param name="channel"></param>
        /// <response code="200">Ok</response>
        /// <response code="400">Channel validation failed</response>
        /// <response code="401">Unauthorize</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedChannelViewModel channel)
        {
            var user = await _userService.GetCurrentUser();
            var channelEntity = channel.ToEntity();
            channelEntity.UserId = user.Id;
            await _channelService.Add(channelEntity);

            return Ok();
        }

        /// <summary>
        /// Update channel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channel"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Channel validation failed</response>
        /// <response code="401">Unauthorize</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] FeedChannelViewModel channel)
        {
            var user = await _userService.GetCurrentUser();

            var channelInStore = await _channelService.Get(id);
            if (!channelInStore.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var channelEntity = channel.ToEntity();
            channelEntity.Id = channelInStore.Id;
            channelEntity.UserId = channelInStore.UserId;
            await _channelService.Update(user, channelEntity);
            return Ok();
        }

        /// <summary>
        /// Delete channel by id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
             if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Forbid();
            await _channelService.Remove(user, channel);
#pragma warning disable 4014
            _feedService.RemoveFeedByChannel(user, channel);
#pragma warning restore 4014
            return Ok();
        }
    }
}