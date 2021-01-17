using Influunt.Feed.Entity;
using Skidbladnir.Repository.MongoDB;

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