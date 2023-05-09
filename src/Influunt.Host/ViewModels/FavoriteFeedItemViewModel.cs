using System;

namespace Influunt.Host.ViewModels
{
    /// <summary>
    /// Favorite News feed item
    /// </summary>
    public class FavoriteFeedItemViewModel
    {
        /// <summary>
        /// News title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Link to news
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// News description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Channel name where news published
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// News Unique identifier in database
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// News Unique identifier
        /// </summary>
        public string ItemHash {get; set;}
    }
}