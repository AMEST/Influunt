using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.Entity;
using Influunt.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using Skidbladnir.Modules;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage
{
    public class StorageModule : Module
    {
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
            });

            //Services
            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IFavoriteFeedService, FavoriteService>();
        }
    }
}