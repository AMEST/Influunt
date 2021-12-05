using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Microsoft.Extensions.Logging;

namespace Influunt.Feed.Rss
{
    internal class RssClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RssClient> _logger;

        public RssClient(HttpClient httpClient, ILogger<RssClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<FeedItem>> GetFeed(FeedChannel channel)
        {
            try
            {
                using (var result = await _httpClient.GetAsync(channel.Url))
                {
                    var xmlRss = await result.Content.ReadAsStringAsync();
                    return xmlRss.IsAtomRss()
                        ? xmlRss.FeedFromAtomRss(channel)
                        : xmlRss.FeedFromRss(channel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Can not get rss feed from \nchannel: {channelName}\nurl: {channelUrl}\n with error: {message} ",
                    channel.Name, channel.Url, e.Message);
                return new List<FeedItem>();
            }
        }
    }
}