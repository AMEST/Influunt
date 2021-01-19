using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
        public string Date { get; set; }
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
        public string ItemHash => ComputeHash();

        private string ComputeHash()
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{Title}.{Description}.{ChannelName}"));
            return string.Concat(hash.Select(b => b.ToString("X2")));
        }
    }
}