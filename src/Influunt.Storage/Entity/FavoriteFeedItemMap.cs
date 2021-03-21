using Influunt.Feed.Entity;
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