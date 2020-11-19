using System;

namespace Influunt.Feed.Rss
{
    public interface IRssFeedServiceConfiguration
    {
        TimeSpan FeedUpdatePeriod { get; set; }
        double LastActivityDaysAgo { get; set; }
    }
}