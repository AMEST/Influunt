using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed;
using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using MongoDB.Bson;

namespace Influunt.MongoStorage.Services
{
    public class FavoriteService: IFavoriteFeedService
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
                Id = ObjectId.GenerateNewId().ToString()
            };
            await _favoriteRepository.Create(favoriteItem);
            return favoriteItem;
        }

        public Task Remove(User user, FavoriteFeedItem favorite)
        {
            if(!favorite.UserId.Equals(user.Id,StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            
            _favoriteRepository.Delete(favorite.Id);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<FavoriteFeedItem>> GetUserFavorites(User user)
        {
            return (await _favoriteRepository.Get()).Where(f => f.UserId == user.Id);
        }
    }
}