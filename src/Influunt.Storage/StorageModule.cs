using System;
using System.Threading;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.Entity;
using Influunt.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Skidbladnir.Modules;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage;

public class StorageModule : RunnableModule
{
    public override Type[] DependsModules => [typeof(HashMigrationWorkerModule)];
    public override void Configure(IServiceCollection services)
    {
        // Register conventions
        var pack = new ConventionPack
        {
            new IgnoreIfDefaultConvention(true),
            new IgnoreExtraElementsConvention(true),
        };

        ConventionRegistry.Register("Influunt", pack, t => true);

        var storageCfg = Configuration.Get<StorageConfiguration>();

        //Database
        services.AddMongoDbContext(builder =>
        {
            builder.UseConnectionString(storageCfg.ConnectionString);
            builder.AddEntity<User, UserMap>();
            builder.AddEntity<FeedChannel, FeedChannelMap>();
            builder.AddEntity<FavoriteFeedItem, FavoriteFeedItemMap>();
            builder.AddEntity<FeedItem, FeedItemMap>();
        });

        //Services
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IChannelService, ChannelService>();
        services.AddSingleton<IFavoriteFeedService, FavoriteService>();
        services.AddSingleton<IFeedService, FeedService>();
    }

    public override async Task StartAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var logger = provider.GetService<ILogger<StorageModule>>();
        var baseMongoContext = provider.GetService<BaseMongoDbContext>();

        logger?.LogInformation("Initialize indices");
        try
        {
            await CreateUserIndexes(baseMongoContext);
            await CreateChannelIndexes(baseMongoContext);
            await CreateFavoriteIndexes(baseMongoContext);
            await CreateFeedIndexes(baseMongoContext);
        }
        catch (Exception e)
        {
            logger?.LogError(e, "Can't create indices");
        }
    }

    private static async Task CreateFeedIndexes(BaseMongoDbContext baseMongoContext)
    {
        var collection = baseMongoContext.GetCollection<FeedItem>();
        var userIdAndChannelIdAndPubDateDefinition = Builders<FeedItem>.IndexKeys
            .Ascending(x => x.UserId)
            .Ascending(x => x.ChannelId)
            .Descending(x => x.PubDate);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<FeedItem>(userIdAndChannelIdAndPubDateDefinition, new CreateIndexOptions()
        {
            Background = true
        }));

        var userIdAndHashDefinition = Builders<FeedItem>.IndexKeys
            .Ascending(x => x.UserId)
            .Ascending(x => x.Hash);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<FeedItem>(userIdAndHashDefinition, new CreateIndexOptions()
        {
            Background = true
        }));
    }

    private static async Task CreateFavoriteIndexes(BaseMongoDbContext baseMongoContext)
    {
        var collection = baseMongoContext.GetCollection<FavoriteFeedItem>();
        var userIdAndPubDateDefinition = Builders<FavoriteFeedItem>.IndexKeys
            .Ascending(x => x.UserId)
            .Descending(x => x.PubDate);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<FavoriteFeedItem>(userIdAndPubDateDefinition, new CreateIndexOptions()
        {
            Background = true
        }));
    }

    private static async Task CreateChannelIndexes(BaseMongoDbContext baseMongoContext)
    {
        var collection = baseMongoContext.GetCollection<FeedChannel>();
        var userIdKeyDefinition = Builders<FeedChannel>.IndexKeys.Ascending(x => x.UserId);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<FeedChannel>(userIdKeyDefinition, new CreateIndexOptions()
        {
            Background = true
        }));
    }

    private static async Task CreateUserIndexes(BaseMongoDbContext baseMongoContext)
    {
        var collection = baseMongoContext.GetCollection<User>();
        var emailKeyDefinition = Builders<User>.IndexKeys.Ascending(x => x.Email);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<User>(emailKeyDefinition, new CreateIndexOptions()
        {
            Unique = true,
            Background = true
        }));
    }
}