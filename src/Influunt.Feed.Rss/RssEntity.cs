using System.Collections.Generic;
using System.Xml.Serialization;
namespace Influunt.Feed.Rss;

[XmlRoot(ElementName = "image")]
public class Image
{
    [XmlElement(ElementName = "link")]
    public string Link { get; set; }
    [XmlElement(ElementName = "url")]
    public string Url { get; set; }
    [XmlElement(ElementName = "title")]
    public string Title { get; set; }
}

[XmlRoot(ElementName = "guid")]
public class Guid
{
    [XmlAttribute(AttributeName = "isPermaLink")]
    public string IsPermaLink { get; set; }
    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "item")]
public class Item
{
    [XmlElement(ElementName = "title")]
    public string Title { get; set; }
    [XmlElement(ElementName = "guid")]
    public Guid Guid { get; set; }
    [XmlElement(ElementName = "link")]
    public string Link { get; set; }
    [XmlElement(ElementName = "description")]
    public string Description { get; set; }
    [XmlElement(ElementName = "pubDate")]
    public string PubDate { get; set; }
    [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
    public string Creator { get; set; }
    [XmlElement(ElementName = "category")]
    public List<string> Category { get; set; }
}

[XmlRoot(ElementName = "channel")]
public class Channel
{
    [XmlElement(ElementName = "title")]
    public string Title { get; set; }
    [XmlElement(ElementName = "link")]
    public string Link { get; set; }
    [XmlElement(ElementName = "description")]
    public string Description { get; set; }
    [XmlElement(ElementName = "language")]
    public string Language { get; set; }
    [XmlElement(ElementName = "managingEditor")]
    public string ManagingEditor { get; set; }
    [XmlElement(ElementName = "generator")]
    public string Generator { get; set; }
    [XmlElement(ElementName = "pubDate")]
    public string PubDate { get; set; }
    [XmlElement(ElementName = "image")]
    public Image Image { get; set; }
    [XmlElement(ElementName = "item")]
    public List<Item> Item { get; set; }
}

[XmlRoot(ElementName = "rss")]
public class RssBody
{
    [XmlElement(ElementName = "channel")]
    public Channel Channel { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }
    [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Dc { get; set; }
}