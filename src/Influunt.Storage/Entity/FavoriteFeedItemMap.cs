using Influunt.Feed.Entity;
using Influunt.Storage.Mongo;

namespace Influunt.Storage.Entity
{
    public class FavoriteFeedItemMap: EntityMapClass<FavoriteFeedItem>
    {
        public FavoriteFeedItemMap()
        {
            ToCollection("favorite");
        }
    }
}