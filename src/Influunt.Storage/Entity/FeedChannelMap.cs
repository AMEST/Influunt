using Influunt.Feed.Entity;
using Influunt.Storage.Mongo;

namespace Influunt.Storage.Entity
{
    public class FeedChannelMap: EntityMapClass<FeedChannel>
    {
        public FeedChannelMap()
        {
            ToCollection("channels");
        }
    }
}