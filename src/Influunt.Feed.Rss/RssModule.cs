using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Influunt.Feed.Rss
{
    public static class RssModule
    {
        public static IServiceCollection AddRssModule(this IServiceCollection services,
            IConfigurationSection configurationSection)
        {
            var configuration = new RssFeedServiceConfiguration();
            configurationSection?.Bind(configuration);
            services.AddSingleton<IRssFeedServiceConfiguration>(configuration);
            return services.AddRssModule();
        }

        public static IServiceCollection AddRssModule(this IServiceCollection services,
            IRssFeedServiceConfiguration configuration)
        {
            services.AddSingleton<IRssFeedServiceConfiguration>(configuration);
            return services.AddRssModule();
        }

        private static IServiceCollection AddRssModule(this IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddScoped<IFeedService, RssFeedService>();
            services.AddHostedService<FeedUpdateWorker>();
            services.AddMemoryCache();
            return services;
        }
    }
}