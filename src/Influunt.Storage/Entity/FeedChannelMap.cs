using Influunt.Feed.Entity;
using MongoDB.Bson;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage.Entity
{
    public class FeedChannelMap : EntityMapClass<FeedChannel>
    {
        public FeedChannelMap()
        {
            ToCollection("FeedChannel");
            MapId(x => x.Id, BsonType.String);
        }
    }
}