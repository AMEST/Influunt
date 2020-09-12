using Influunt.Feed.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

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