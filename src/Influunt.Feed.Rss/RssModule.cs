using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Influunt.Feed.Rss
{
    public static class RssModule
    {
        public static IServiceCollection AddRssModule(this IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddScoped<IFeedService, RssFeedService>();
            services.AddMemoryCache();
            return services;
        }
    }
}