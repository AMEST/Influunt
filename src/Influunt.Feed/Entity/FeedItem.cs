using System;
using System.Text.RegularExpressions;
using System.Web;
using Skidbladnir.Repository.Abstractions;

namespace Influunt.Feed.Entity
{
    public class FeedItem : IHasId<string>
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Hash { get; set; }

        public FeedItem NormalizeDescription()
        {
            if (Description == null) return this;

            Description = HttpUtility.HtmlDecode(Description);
            Description = Regex.Replace(Description, @"\<script.*\<\/script\>", "", RegexOptions.IgnoreCase);
            Description = Regex.Replace(Description, @"\<style.*\<\/style\>", "", RegexOptions.IgnoreCase);
            Description = Regex.Replace(Description, @"\<iframe.*\<\/iframe\>", "", RegexOptions.IgnoreCase);
            Description = Regex.Replace(Description, @"\<frame.*\<\/frame\>", "", RegexOptions.IgnoreCase);
            Description = Regex.Replace(Description, @"\<frameset.*\<\/frameset\>", "", RegexOptions.IgnoreCase);
            return this;
        }
    }
}