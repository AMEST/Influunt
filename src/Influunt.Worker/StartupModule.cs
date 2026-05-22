using Influunt.Feed.Extensions;
using Influunt.Feed.Rss;
using Influunt.Storage;
using Microsoft.AspNetCore.Http;
using Skidbladnir.Caching.Distributed.MongoDB;
using Skidbladnir.Modules;

namespace Influunt.Worker;

public class StartupModule : Module
{
    public override Type[] DependsModules => [typeof(StorageModule), typeof(RssModule)];

    public override void Configure(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, NoOpHttpContextAccessor>();
        ConfigureDistributedCache(services);
        services.AddFeedJobs();
    }

        private void ConfigureDistributedCache(IServiceCollection services)
    {
        var redisConfiguration = Configuration.AppConfiguration["ConnectionStrings:Redis:ConnectionString"];
        if(string.IsNullOrWhiteSpace(redisConfiguration))
        {
            services.AddMongoDistributedCache(Configuration.AppConfiguration["ConnectionStrings:Mongo:ConnectionString"]);
        }
        else
        {
            services.AddStackExchangeRedisCache(c => c.Configuration = redisConfiguration);
        }       
    }
}