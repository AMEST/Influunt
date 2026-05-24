namespace Influunt.Feed.Crawler;

public class CrawlerOptions
{
    public int LastActivityDaysAgo { get; set; } = 31*3; // 3 Month default
}