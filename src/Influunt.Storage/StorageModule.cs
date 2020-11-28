using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.DataProtection;
using Influunt.Storage.Entity;
using Influunt.Storage.Mongo;
using Influunt.Storage.Mongo.Abstractions;
using Influunt.Storage.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Influunt.Storage
{
    public static class StorageModule
    {
        public static IServiceCollection AddStorage(this IServiceCollection services,
            IMongoStorageConfiguration configuration)
        {

            services.AddMongoModule(configuration);

            services.AddMongoRepository<User, UserMap>();
            services.AddMongoRepository<FeedChannel, FeedChannelMap>();
            services.AddMongoRepository<FavoriteFeedItem, FavoriteFeedItemMap>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IFavoriteFeedService, FavoriteService>();

            return services;
        }

        public static IServiceCollection AddDataProtectionStore(this IServiceCollection services)
        {
            return services
                .AddMongoRepository<DbXmlKey, DbXmlKeyMap>()
                .AddSingleton<IXmlRepository, MongoDbXmlRepository>()
                .AddSingleton<IConfigureOptions<KeyManagementOptions>, DataProtectionOptionsConfigurator>();
        }
    }
}