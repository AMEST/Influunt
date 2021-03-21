using System;
using Influunt.Feed.Rss;
using Influunt.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skidbladnir.Caching.Distributed.MongoDB;
using Skidbladnir.DataProtection.MongoDb;
using Skidbladnir.Modules;

namespace Influunt.Host
{
    public class StartupModule: Module
    {
        public override Type[] DependsModules => new[] {typeof(StorageModule), typeof(RssModule)};

        public override void Configure(IServiceCollection services)
        {
            services.UseDataProtection();
            services.UseMongoDistributedCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}