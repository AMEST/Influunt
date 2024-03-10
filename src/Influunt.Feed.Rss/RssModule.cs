using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Skidbladnir.Modules;

namespace Influunt.Feed.Rss;

public class RssModule : Module
{
    public override void Configure(IServiceCollection services)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        services.AddHttpClient<RssClient>();
        services.AddSingleton<IFeedSourceProvider, RssFeedSourceProvider>();
    }
}