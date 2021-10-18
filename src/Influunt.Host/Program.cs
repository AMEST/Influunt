using System.Threading.Tasks;
using Influunt.Feed.Rss;
using Influunt.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                .UseSkidbladnirModules<StartupModule>(configuration =>
                {
                    var rssFeedConfiguration = configuration.AppConfiguration.GetSection("FeedService")
                        .Get<RssFeedServiceConfiguration>();
                    configuration.Add(rssFeedConfiguration);
                    var storageConfiguration = configuration.AppConfiguration.GetSection("ConnectionStrings:Mongo").Get<StorageConfiguration>();
                    configuration.Add(storageConfiguration);
                });
    }
}
