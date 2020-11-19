using System;

namespace Influunt.Feed.Rss
{
    public class RssFeedServiceConfiguration : IRssFeedServiceConfiguration
    {
        public TimeSpan FeedUpdatePeriod { get; set; } = TimeSpan.FromMinutes(30);
        public double LastActivityDaysAgo { get; set; } = 2;
    }
}