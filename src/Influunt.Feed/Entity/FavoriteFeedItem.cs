using Skidbladnir.Repository.Abstractions;

namespace Influunt.Feed.Entity
{
    public class FavoriteFeedItem : FeedItem, IHasId<string>
    {
        public string UserId { get; set; }
        public string Id { get; set; }
    }
}