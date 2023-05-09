using System;
using System.Text;
using Force.Crc32;
using Influunt.Feed.Entity;

namespace Influunt.Feed.Services
{
    public static class HashExtensions
    {
        private static readonly Crc32Algorithm _crc32 = new Crc32Algorithm();

        public static string ComputeHash(this FeedItem item)
        {
            var data = $"{item.Title}.{item.Description}.{item.ChannelId}";
            return data.ComputeHash();
        }

        public static string ComputeHash(this string data){
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = _crc32.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}