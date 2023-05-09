using System;

namespace Influunt.Feed.Crawler
{
    public class CrawlerOptions
    {
        public TimeSpan FetchInterval { get; set; } = TimeSpan.FromMinutes(30);
        public int LastActivityDaysAgo { get; set; } = 31*3; // 3 Month default
    }
}