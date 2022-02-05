namespace Tumblepub.ActivityPub.Models;

public class WebfingerActor
{
    public string Subject { get; set; } = string.Empty;
    public List<WebfingerLink> Links { get; set; } = new();
}

public class WebfingerLink
{
    public string Rel { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}
