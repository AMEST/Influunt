using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Microsoft.Extensions.Logging;

namespace Influunt.Feed.Rss;

internal class RssClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RssClient> _logger;

    public RssClient(HttpClient httpClient, ILogger<RssClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<FeedItem>> GetFeed(FeedChannel channel)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, channel.Url);
            request.Headers.Add("user-agent", UserAgents.Generate());
            using (var result = await _httpClient.SendAsync(request))
            {
                var xmlRss = await result.Content.ReadAsStringAsync();
                return xmlRss.IsAtomRss()
                    ? xmlRss.FeedFromAtomRss()
                    : xmlRss.FeedFromRss();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Can not get rss feed from \nchannel: {channelName}\nurl: {channelUrl}\n with error: {message} ",
                channel.Name, channel.Url, e.Message);
            return Array.Empty<FeedItem>();
        }
    }
}