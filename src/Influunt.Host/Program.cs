using System.Threading.Tasks;
using Influunt.Feed.Crawler;
using Influunt.Host.Configurations;
using Influunt.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skidbladnir.Modules;

namespace Influunt.Host
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((ctx, services) => {
                    services.AddOptions();
                    services.Configure<CrawlerOptions>(ctx.Configuration.GetSection("FeedCrawler"));
                    var crawlerEnabled = ctx.Configuration.GetSection("FeedCrawler:Enabled").Get<bool>();
                    if(crawlerEnabled)
                        services.AddHostedService<FeedCrawlerBackgroundWorker>();
                })
                .UseSkidbladnirModules<StartupModule>(configuration =>
                {
                    var storageConfiguration = configuration.AppConfiguration.GetSection("ConnectionStrings:Mongo").Get<StorageConfiguration>();
                    configuration.Add(storageConfiguration);
                    var redisConfiguration = configuration.AppConfiguration.GetSection("ConnectionStrings:Redis").Get<RedisConfiguration>();
                    configuration.Add(redisConfiguration);
                });
    }
}
