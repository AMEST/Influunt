using System.Collections.Generic;
using System.Xml.Serialization;

namespace Influunt.Feed.Rss;

[XmlRoot(ElementName = "generator", Namespace = "http://www.w3.org/2005/Atom")]
public class Generator
{
    [XmlAttribute(AttributeName = "uri")]
    public string Uri { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }
    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
public class Link
{
    [XmlAttribute(AttributeName = "href")]
    public string Href { get; set; }
    [XmlAttribute(AttributeName = "rel")]
    public string Rel { get; set; }
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; }
}

[XmlRoot(ElementName = "content", Namespace = "http://www.w3.org/2005/Atom")]
public class Content
{
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "base", Namespace = "http://www.w3.org/XML/1998/namespace")]
    public string Base { get; set; }
    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "category", Namespace = "http://www.w3.org/2005/Atom")]
public class Category
{
    [XmlAttribute(AttributeName = "term")]
    public string Term { get; set; }
}

[XmlRoot(ElementName = "entry", Namespace = "http://www.w3.org/2005/Atom")]
public class Entry
{
    [XmlElement(ElementName = "title", Namespace = "http://www.w3.org/2005/Atom")]
    public string Title { get; set; }
    [XmlElement(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
    public Link Link { get; set; }
    [XmlElement(ElementName = "published", Namespace = "http://www.w3.org/2005/Atom")]
    public string Published { get; set; }
    [XmlElement(ElementName = "updated", Namespace = "http://www.w3.org/2005/Atom")]
    public string Updated { get; set; }
    [XmlElement(ElementName = "id", Namespace = "http://www.w3.org/2005/Atom")]
    public string Id { get; set; }
    [XmlElement(ElementName = "content", Namespace = "http://www.w3.org/2005/Atom")]
    public Content Content { get; set; }
    [XmlElement(ElementName = "category", Namespace = "http://www.w3.org/2005/Atom")]
    public Category Category { get; set; }
    [XmlElement(ElementName = "summary", Namespace = "http://www.w3.org/2005/Atom")]
    public string Summary { get; set; }
}

[XmlRoot(ElementName = "feed", Namespace = "http://www.w3.org/2005/Atom")]
public class Feed
{
    [XmlElement(ElementName = "generator", Namespace = "http://www.w3.org/2005/Atom")]
    public Generator Generator { get; set; }
    [XmlElement(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
    public List<Link> Link { get; set; }
    [XmlElement(ElementName = "updated", Namespace = "http://www.w3.org/2005/Atom")]
    public string Updated { get; set; }
    [XmlElement(ElementName = "id", Namespace = "http://www.w3.org/2005/Atom")]
    public string Id { get; set; }
    [XmlElement(ElementName = "entry", Namespace = "http://www.w3.org/2005/Atom")]
    public List<Entry> Entry { get; set; }
    [XmlAttribute(AttributeName = "xmlns")]
    public string Xmlns { get; set; }
}