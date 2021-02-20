using Influunt.Feed;
using Influunt.Feed.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Storage.Services
{
    public class FavoriteService : IFavoriteFeedService
    {
        private readonly IRepository<FavoriteFeedItem> _favoriteRepository;

        public FavoriteService(IRepository<FavoriteFeedItem> favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<FavoriteFeedItem> Add(User user, FeedItem favorite)
        {
            var favoriteItem = new FavoriteFeedItem
            {
                PubDate = favorite.PubDate,
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

        public Task<IEnumerable<FavoriteFeedItem>> GetUserFavorites(User user, int? offset)
        {
            return Task.Run(() => GetFavorites(user,offset).ToList().AsEnumerable());
        }

        private IQueryable<FavoriteFeedItem> GetFavorites(User user, int? offset)
        {
            var query = _favoriteRepository.GetAll()
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.PubDate);

            if (offset == null)
                return query;

            return query.Skip(offset.Value).Take(10);
        }
    }
}