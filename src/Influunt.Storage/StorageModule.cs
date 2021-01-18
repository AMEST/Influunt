using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.Entity;
using Influunt.Storage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skidbladnir.Caching.Distributed.MongoDB;
using Skidbladnir.DataProtection.MongoDb;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage
{
    public static class StorageModule
    {
        public static IServiceCollection AddStorage(this IServiceCollection services,
            StorageConfiguration configuration)
        {
            services.AddMongoDbContext(builder =>
                {
                    builder.UseConnectionString(configuration.ConnectionString);
                    builder.AddEntity<User, UserMap>();
                    builder.AddEntity<FeedChannel, FeedChannelMap>();
                    builder.AddEntity<FavoriteFeedItem, FavoriteFeedItemMap>();
                    builder.UseDataProtection(services);
                    builder.UseMongoDistributedCache(services);
                });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IFavoriteFeedService, FavoriteService>();

            return services;
        }
    }
}