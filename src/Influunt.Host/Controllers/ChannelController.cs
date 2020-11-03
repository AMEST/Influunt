using System;
using Influunt.Feed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Influunt.Feed.Entity;

namespace Influunt.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IChannelService _channelService;

        public ChannelController(IUserService userService, IChannelService channelService)
        {
            _userService = userService;
            _channelService = channelService;
        }

        [HttpGet]
        public async Task<IEnumerable<FeedChannel>> Get()
        {
            var user = await _userService.GetCurrentUser();
            return await _channelService.GetUserChannels(user);
        }

        [HttpGet("{id}")]
        public async Task<FeedChannel> Get(string id)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            if (channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return channel;

            Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedChannel channel)
        {
            var user = await _userService.GetCurrentUser();
            if (!ValidateChannel(channel))
                return BadRequest();

            channel.UserId = user.Id;
            await _channelService.Add(channel);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] FeedChannel channel)
        {
            var user = await _userService.GetCurrentUser();
            if (!ValidateChannel(channel))
                return BadRequest();

            var channelInStore = await _channelService.Get(id);
            if (!channelInStore.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
            {
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return BadRequest();
            }

            channel.Id = channelInStore.Id;
            channel.UserId = channelInStore.UserId;
            await _channelService.Update(user, channel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            await _channelService.Remove(user, channel);
        }

        private static bool ValidateChannel(FeedChannel channel)
        {
            if (string.IsNullOrWhiteSpace(channel.Name)
                || string.IsNullOrWhiteSpace(channel.Url)
                ||(!channel.Url.StartsWith("http://")
                && !channel.Url.StartsWith("https://")))
                return false;

            return true;
        }
    }
}