using System.Threading.Tasks;
using Influunt.Feed.Crawler;
using Microsoft.Extensions.DependencyInjection;

namespace Influunt.Feed.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddFeedJobs(this IServiceCollection services)
    {
        return services.AddScoped<FeedCrawlerJob>()
                .AddScoped<UserFeedChannelUpdateJob>()
                .AddHostedService<FeedJobsRegistrationService>();
    }
}
