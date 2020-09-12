using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using Influunt.MongoStorage.Entity;
using Influunt.MongoStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Influunt.MongoStorage
{
    public static class MongoModule
    {
        public static IServiceCollection AddMongoStorage(this IServiceCollection services,
            IMongoStorageConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            services.AddMongoRepository<User, UserMap>();
            services.AddMongoRepository<FeedChannel, FeedChannelMap>();
            services.AddMongoRepository<FavoriteFeedItem, FavoriteFeedItemMap>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IFavoriteFeedService, FavoriteService>();

            return services;
        }
    }
}