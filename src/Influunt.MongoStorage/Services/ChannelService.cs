﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using MongoDB.Bson;

namespace Influunt.MongoStorage.Services
{
    public class ChannelService: IChannelService
    {
        private readonly IMongoRepository<FeedChannel> _channelRepository;

        public ChannelService(IMongoRepository<FeedChannel> channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public Task<FeedChannel> Get(string id)
        {
            return _channelRepository.Get(id);
        }

        public async Task<FeedChannel> Add(FeedChannel channel)
        {
            channel.Id = ObjectId.GenerateNewId().ToString();

            await _channelRepository.Create(channel);

            return channel;

        }

        public Task Remove(User user, FeedChannel channel)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            _channelRepository.Delete(channel.Id);
            return Task.CompletedTask;
        }

        public Task Update(User user, FeedChannel channel)
        {
            if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            _channelRepository.Update(channel);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<FeedChannel>> GetUserChannels(User user)
        {
            return (await _channelRepository.Get()).Where(c => c.UserId == user.Id);
        }
    }
}