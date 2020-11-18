using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Influunt.Feed.Rss
{
    public class FeedUpdatingWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FeedUpdatingWorker> _logger;
        private readonly IRssFeedServiceConfiguration _configuration;
        private Timer _timer;

        public FeedUpdatingWorker(IServiceProvider serviceProvider, ILogger<FeedUpdatingWorker> logger,
            IRssFeedServiceConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                _configuration.FeedUpdatePeriod);
            _logger.LogInformation("Starting Feed Updater with update period {UpdatePeriod}",
                _configuration.FeedUpdatePeriod);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            return base.StopAsync(cancellationToken);
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetService<IUserService>();
                var feedService = scope.ServiceProvider.GetService<IFeedService>();
                var allUsers = userService.GetUsers().GetAwaiter().GetResult();

                var updateTasks = allUsers.Where(u =>
                        u.LastActivity > DateTime.UtcNow.AddDays(-_configuration.LastActivityDaysAgo))
                    .Select(user => feedService.GetFeed(user))
                    .Cast<Task>()
                    .ToArray();

                _logger.LogInformation("Count active users ({DaysAgo} days) for update feed: {ActiveUsers}",
                    _configuration.LastActivityDaysAgo, updateTasks.Length);
                Task.WaitAll(updateTasks);
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}