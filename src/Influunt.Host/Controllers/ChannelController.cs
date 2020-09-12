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
        public async void Post([FromBody] FeedChannel channel)
        {
            var user = await _userService.GetCurrentUser();
            channel.UserId = user.Id;
            await _channelService.Add(channel);
        }

        [HttpPut("{id}")]
        public async void Put(string id, [FromBody] FeedChannel channel)
        {
            var user = await _userService.GetCurrentUser();
            var channelInStore = await _channelService.Get(id);
            if (!channelInStore.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
            {
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return;
            }

            channel.Id = channelInStore.Id;
            channel.UserId = channelInStore.UserId;
            await _channelService.Update(user, channel);
        }

        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            var user = await _userService.GetCurrentUser();
            var channel = await _channelService.Get(id);
            await _channelService.Remove(user, channel);
        }
    }
}