using Influunt.Feed;
using Influunt.Feed.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Storage.Services
{
    internal class ChannelService : IChannelService
    {
        private readonly IRepository<FeedChannel> _channelRepository;

        public ChannelService(IRepository<FeedChannel> channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public Task<FeedChannel> Get(string id)
        {
            return _channelRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<FeedChannel> Add(FeedChannel channel)
        {
            await _channelRepository.Create(channel);
            return channel;
        }

        public Task Remove(User user, FeedChannel channel)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            return _channelRepository.Delete(channel);
        }

        public Task Update(User user, FeedChannel channel)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            return _channelRepository.Update(channel);
        }

        public async Task<IEnumerable<FeedChannel>> GetUserChannels(User user)
        {
            return await _channelRepository.GetAll().Where(c => c.UserId == user.Id).ToArrayAsync();
        }
    }
}