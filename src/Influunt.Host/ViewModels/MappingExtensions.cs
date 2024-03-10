using System.Collections.Generic;
using System.Linq;
using Influunt.Feed.Entity;

namespace Influunt.Host.ViewModels;

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
        if (user is null)
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
    public static IEnumerable<FeedItemViewModel> ToModel(this IEnumerable<FeedItem> feedItem, IEnumerable<FeedChannel> channels)
    {
        return feedItem.Select(x => new FeedItemViewModel
        {
            ChannelName = channels.FirstOrDefault(c => c.Id == x.ChannelId)?.Name,
            Date = x.PubDate,
            Description = x.Description,
            Link = x.Link,
            Title = x.Title,
            ItemHash = x.Hash
        });
    }

    /// <summary>
    /// FeedItemViewModel to FavoriteFeedItem mapping
    /// </summary>
    public static FavoriteFeedItem ToEntity(this FeedItemViewModel feedViewItem)
    {
        return new FavoriteFeedItem
        {
            PubDate = feedViewItem.Date,
            Description = feedViewItem.Description,
            Link = feedViewItem.Link,
            Title = feedViewItem.Title,
            Hash = feedViewItem.ItemHash,
            ChannelName = feedViewItem.ChannelName
        };
    }

    /// <summary>
    /// FavoriteFeedItem to FavoriteFeedItemViewModel mapping
    /// </summary>
    public static IEnumerable<FavoriteFeedItemViewModel> ToModel(this IEnumerable<FavoriteFeedItem> feedItem)
    {
        return feedItem.Select(x => new FavoriteFeedItemViewModel
        {
            Date = x.PubDate,
            Description = x.Description,
            Link = x.Link,
            Title = x.Title,
            Id = x.Id,
            ItemHash = x.Hash,
            ChannelName = x.ChannelName
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