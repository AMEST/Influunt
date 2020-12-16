using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.Storage.Mongo.Abstractions;
using MongoDB.Bson;

namespace Influunt.Storage.Services
{
    public class FavoriteService : IFavoriteFeedService
    {
        private readonly IMongoRepository<FavoriteFeedItem> _favoriteRepository;

        public FavoriteService(IMongoRepository<FavoriteFeedItem> favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<FavoriteFeedItem> Add(User user, FeedItem favorite)
        {
            var favoriteItem = new FavoriteFeedItem
            {
                Date = favorite.Date,
                Description = favorite.Description,
                Link = favorite.Link,
                Title = favorite.Title,
                UserId = user.Id,
                ChannelName = favorite.ChannelName,
                Id = ObjectId.GenerateNewId().ToString()
            };
            await _favoriteRepository.Create(favoriteItem);
            return favoriteItem;
        }

        public async Task Remove(User user, string id)
        {
            var favorite = await _favoriteRepository.Get(id);
            if (favorite == null || !favorite.UserId.Equals(user.Id, StringComparison.OrdinalIgnoreCase))
                return;

            _favoriteRepository.Delete(favorite.Id);
        }

        public Task<IEnumerable<FavoriteFeedItem>> GetUserFavorites(User user)
        {
            return Task.Run(() => _favoriteRepository.GetAll().Where(f => f.UserId == user.Id).ToList().AsEnumerable());
        }
    }
}