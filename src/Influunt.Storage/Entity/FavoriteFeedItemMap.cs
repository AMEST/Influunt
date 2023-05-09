using Influunt.Feed.Entity;
using MongoDB.Bson;
using Skidbladnir.Repository.MongoDB;

namespace Influunt.Storage.Entity
{
    public class FavoriteFeedItemMap : EntityMapClass<FavoriteFeedItem>
    {
        public FavoriteFeedItemMap()
        {
            ToCollection("FavoriteFeedItem");
        }
    }
}