using System.Collections.Generic;
using System.Linq;
using Influunt.Feed.Entity;

namespace Influunt.Host.ViewModels
{
    public static class MappingExtensions
    {
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