using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Skidbladnir.Modules;

namespace Influunt.Feed.Rss
{
    public class RssModule : RunnableModule
    {
        public override void Configure(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var configuration = Configuration.GetSection("FeedService").Get<RssFeedServiceConfiguration>();
            services.AddSingleton<IRssFeedServiceConfiguration>(configuration);
            services.AddScoped<IFeedService, RssFeedService>();
            services.AddHostedService<FeedUpdateWorker>();
        }
    }
}