using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Skidbladnir.Modules;

namespace Influunt.Feed.Rss
{
    public class RssModule : ScheduledModule
    {
        private RssFeedServiceConfiguration _moduleConfiguration;
        public override void Configure(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _moduleConfiguration = Configuration.Get<RssFeedServiceConfiguration>();
            services.AddSingleton<RssFeedServiceConfiguration>(_moduleConfiguration);
            services.AddHttpClient<RssClient>();
            services.AddScoped<IFeedService, RssFeedService>();
        }

        public override async Task ExecuteAsync(IServiceProvider provider, CancellationToken cancellationToken = new CancellationToken())
        {
            var logger = provider.GetService<ILogger<RssModule>>();
            logger.LogInformation("Start Feed Update");
            using (var scope = provider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetService<IUserService>();
                var feedService = scope.ServiceProvider.GetService<IFeedService>();
                var allUsers = await userService.GetUsers();

                var updateTasks = allUsers.Where(u =>
                        u.LastActivity > DateTime.UtcNow.AddDays(-_moduleConfiguration.LastActivityDaysAgo))
                    .Select(user => feedService.GetFeed(user))
                    .Cast<Task>()
                    .ToArray();

                logger.LogInformation("Count active users ({DaysAgo} days) for update feed: {ActiveUsers}",
                    _moduleConfiguration.LastActivityDaysAgo, updateTasks.Length);
                await Task.WhenAll(updateTasks);
                foreach (var updateTask in updateTasks) updateTask.Dispose();
            }
        }

        public override string CronExpression => _moduleConfiguration.FeedUpdateCron;
    }
}