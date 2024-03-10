using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DnsClient.Internal;
using Influunt.Feed.Entity;
using Influunt.Feed.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skidbladnir.Modules;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Storage.Services;

// Add missing hash and channel id for favorites
internal class HashMigrationWorkerModule : BackgroundModule
{
    public override async Task ExecuteAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var logger = provider.GetRequiredService<ILogger<HashMigrationWorkerModule>>();
        var favoriteRepository = provider.GetRequiredService<IRepository<FavoriteFeedItem>>();
        var channelRepository = provider.GetRequiredService<IRepository<FeedChannel>>();
        try
        {
            var favoriteWithoutHash = await favoriteRepository.GetAll().Where(x =>
                string.IsNullOrEmpty(x.Hash)
                || string.IsNullOrEmpty(x.ChannelId))
                .ToArrayAsync();
            logger.LogInformation($"Favorites count without hash = {favoriteWithoutHash.Length}");
            foreach (var item in favoriteWithoutHash)
            {
                if (!string.IsNullOrEmpty(item.ChannelId) && !string.IsNullOrEmpty(item.Hash))
                    continue;
                var channel = await channelRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == item.UserId && x.Name == item.ChannelName);
                item.ChannelId = channel?.Id;
                item.Hash = item.ComputeHash();
                await favoriteRepository.Update(item);
            }
            logger.LogInformation("Fix hashes and channels completed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error when compute missing favorites hashes");
        }
    }
}