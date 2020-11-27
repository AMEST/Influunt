using Influunt.Feed.Entity;

namespace Influunt.MongoStorage.Entity
{
    public class FavoriteFeedItemMap: EntityMapClass<FavoriteFeedItem>
    {
        public FavoriteFeedItemMap()
        {
            ToCollection("favorite");
        }
    }
}