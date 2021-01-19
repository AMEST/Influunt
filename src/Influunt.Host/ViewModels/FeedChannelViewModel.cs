namespace Influunt.Host.ViewModels
{
    /// <summary>
    /// News channel
    /// </summary>
    public class FeedChannelViewModel
    {
        /// <summary>
        /// Url for getting news
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Channel name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Show in infinity feed flag
        /// </summary>
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// Unique Identifier
        /// </summary>
        public string Id { get; set; }
    }
}