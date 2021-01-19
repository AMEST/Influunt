using System.Collections.Generic;
using System.Linq;
using Influunt.Feed.Entity;

namespace Influunt.Host.ViewModels
{
    /// <summary>
    /// Extensions for class mapping
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// User to UseViewModel mapping
        /// </summary>
        public static UserViewModel ToModel(this User user)
        {
            if (user == null)
                return null;

            return new UserViewModel
            {
                Email = user.Email,
                Name = user.Name
            };
        }

        /// <summary>
        /// FeedItem to FeedItemViewModel mapping
        /// </summary>
        public static IEnumerable<FeedItemViewModel> ToModel(this IEnumerable<FeedItem> feedItem)
        {
            return feedItem.Select(x => new FeedItemViewModel
            {
                ChannelName = x.ChannelName,
                Date = x.Date,
                Description = x.Description,
                Link = x.Link,
                Title = x.Title
            });
        }

        /// <summary>
        /// FeedItemViewModel to FeedItem mapping
        /// </summary>
        public static FeedItem ToEntity(this FeedItemViewModel feedViewItem)
        {
            return new FeedItem
            {
                ChannelName = feedViewItem.ChannelName,
                Date = feedViewItem.Date,
                Description = feedViewItem.Description,
                Link = feedViewItem.Link,
                Title = feedViewItem.Title
            };
        }

        /// <summary>
        /// FavoriteFeedItem to FavoriteFeedItemViewModel mapping
        /// </summary>
        public static IEnumerable<FavoriteFeedItemViewModel> ToModel(this IEnumerable<FavoriteFeedItem> feedItem)
        {
            return feedItem.Select(x => new FavoriteFeedItemViewModel
            {
                ChannelName = x.ChannelName,
                Date = x.Date,
                Description = x.Description,
                Link = x.Link,
                Title = x.Title,
                Id = x.Id
            });
        }

        /// <summary>
        /// FeedChannel to FeedChannelViewModel mapping
        /// </summary>
        public static IEnumerable<FeedChannelViewModel> ToModel(this IEnumerable<FeedChannel> channelItem)
        {
            return channelItem.Select(x => new FeedChannelViewModel
            {
                Hidden = x.Hidden,
                Id = x.Id,
                Name = x.Name,
                Url = x.Url
            });
        }

        /// <summary>
        /// FeedChannel to FeedChannelViewModel mapping
        /// </summary>
        public static FeedChannelViewModel ToModel(this FeedChannel channelItem)
        {
            return new FeedChannelViewModel
            {
                Hidden = channelItem.Hidden,
                Id = channelItem.Id,
                Name = channelItem.Name,
                Url = channelItem.Url
            };
        }

        /// <summary>
        /// FeedChannelViewModel to FeedChannel mapping
        /// </summary>
        public static FeedChannel ToEntity(this FeedChannelViewModel channelItem)
        {
            return new FeedChannel
            {
                Hidden = channelItem.Hidden,
                Id = channelItem.Id,
                Name = channelItem.Name,
                Url = channelItem.Url
            };
        }
    }
}