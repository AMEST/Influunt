using Influunt.Feed.Entity;
using MongoDB.Bson;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage.Entity
{
    public class FeedItemMap  : EntityMapClass<FeedItem>
    {
        public FeedItemMap()
        {
            ToCollection("Feed");
            MapId(x => x.Id, BsonType.String);
        }
    }
}