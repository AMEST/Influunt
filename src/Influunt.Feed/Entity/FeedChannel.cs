using Skidbladnir.Repository.Abstractions;

namespace Influunt.Feed.Entity;

public class FeedChannel : IHasId<string>
{
    public string Url { get; set; }

    public string Name { get; set; }

    public string UserId { get; set; }

    public bool Hidden { get; set; } = false;

    public string Id { get; set; }
}