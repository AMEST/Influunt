using Influunt.Feed.Entity;

namespace Influunt.MongoStorage.Entity
{
    public class FeedChannelMap: EntityMapClass<FeedChannel>
    {
        public FeedChannelMap()
        {
            ToCollection("channels");
        }
    }
}