using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace Influunt.Host.ViewModels
{
    public class FeedItemViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string ChannelName { get; set; }
        public string ItemHash => ComputeHash();

        private string ComputeHash()
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{Title}.{Description}.{ChannelName}"));
            return string.Concat(hash.Select(b => b.ToString("X2")));
        }
    }
}