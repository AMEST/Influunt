namespace Influunt.Feed.Rss
{
    public class RssFeedServiceConfiguration
    {
        public string FeedUpdateCron { get; set; } = "*/30 * * * *";
        public double LastActivityDaysAgo { get; set; } = 2;
    }
}