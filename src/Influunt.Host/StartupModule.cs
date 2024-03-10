using System;
using Influunt.Feed.Rss;
using Influunt.Host.Configurations;
using Influunt.Host.Services;
using Influunt.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skidbladnir.Caching.Distributed.MongoDB;
using Skidbladnir.DataProtection.MongoDb;
using Skidbladnir.Modules;

namespace Influunt.Host;

public class StartupModule : Module
{
    public override Type[] DependsModules => [typeof(WebModule), typeof(StorageModule), typeof(RssModule)];

    public override void Configure(IServiceCollection services)
    {
        services.AddDataProtection()
            .PersistKeysToMongoDb(Configuration.AppConfiguration["ConnectionStrings:Mongo:ConnectionString"]);
        ConfigureDistributedCache(services);
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    private void ConfigureDistributedCache(IServiceCollection services)
    {
        var redisConfiguration = Configuration.Get<RedisConfiguration>();
        if(string.IsNullOrWhiteSpace(redisConfiguration?.ConnectionString))
        {
            services.AddMongoDistributedCache(Configuration.AppConfiguration["ConnectionStrings:Mongo:ConnectionString"]);
        }
        else
        {
            services.AddStackExchangeRedisCache(c => c.Configuration = redisConfiguration.ConnectionString)
                    .Decorate<IDistributedCache, RedisCacheFallbackDecorator>();
        }
            
    }
}