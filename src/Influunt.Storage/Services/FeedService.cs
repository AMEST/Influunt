using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Feed.Services;
using Microsoft.Extensions.Logging;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Storage.Services;

internal class FeedService : IFeedService
{
    private const int BatchSize = 100;
    private readonly IChannelService _channelService;
    private readonly IRepository<FeedItem> _feedRepository;
    private readonly ILogger<FeedService> _logger;

    public FeedService(IChannelService channelService,
        IRepository<FeedItem> feedRepository,
        ILogger<FeedService> logger)
    {
        _channelService = channelService;
        _feedRepository = feedRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FeedItem>> GetFeed(User user, int? offset = null, int count = 10)
    {
        var userChannels = await _channelService.GetUserChannels(user);
        var visibleUserChannels = userChannels.Where(x => !x.Hidden).Select(x => x.Id).ToArray();
        if (visibleUserChannels.Length == 0)
            return Array.Empty<FeedItem>();

        var feed = _feedRepository.GetAll().Where(x => x.UserId == user.Id && visibleUserChannels.Contains(x.ChannelId))
            .OrderByDescending(x => x.PubDate);
        return await feed.GetChunkedFeed(offset, count).ToArrayAsync();

    }

    public async Task<IEnumerable<FeedItem>> GetFeed(User user, FeedChannel channel, int? offset = null, int count = 10)
    {
        if (!channel.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
            return new List<FeedItem>();

        var feed = _feedRepository.GetAll().Where(x => x.UserId == user.Id && x.ChannelId == channel.Id)
            .OrderByDescending(x => x.PubDate);

        return await feed.GetChunkedFeed(offset, count).ToArrayAsync();
    }

    public async Task RemoveFeedByChannel(User user, FeedChannel channel)
    {
        _logger.LogInformation($"Removing channel with id = {channel.Id} and name {channel.Name} from user {user.Id} with all feed items owned to channel");
        var page = 0;
        var channelFeed = await _feedRepository.GetAll()
            .Where(x => x.UserId == user.Id && x.ChannelId == channel.Id)
            .GetChunkedFeed(page * BatchSize, BatchSize)
            .ToArrayAsync();
        while (channelFeed.Length != 0)
        {
            foreach (var item in channelFeed)
                await _feedRepository.Delete(item);
            page++;
            channelFeed = await _feedRepository.GetAll()
                .Where(x => x.UserId == user.Id && x.ChannelId == channel.Id)
                .GetChunkedFeed(page * BatchSize, BatchSize)
                .ToArrayAsync();
        }
        _logger.LogInformation($"Removing channel with id = {channel.Id} and all channel feed completed");
    }

    public async Task<int> TryAddToFeed(User user, IEnumerable<FeedItem> newFeedItems, FeedChannel channel = null)
    {
        var addedCount = 0;
        foreach (var item in newFeedItems)
        {
            item.UserId = user.Id;
            if (channel is not null)
                item.ChannelId = channel.Id;
            item.Hash = item.ComputeHash();
            if (await _feedRepository.GetAll().AnyAsync(x => x.UserId == user.Id && x.Hash == item.Hash))
                continue;
            await _feedRepository.Create(item);
            addedCount++;
        }
        return addedCount;
    }
}