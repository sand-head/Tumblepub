using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.Models;

public class PublicKey
{
    public Uri Id { get; set; } = default!;
    public Uri Owner { get; set; } = default!;
    public string PublicKeyPem { get; set; } = string.Empty;

    internal void AddDomain(Uri domain)
    {
        if (!Id.IsAbsoluteUri)
        {
            Id = new Uri(domain, Id);
        }

        if (!Owner.IsAbsoluteUri)
        {
            Owner = new Uri(domain, Owner);
        }
    }
}

public class Actor
{
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new();
    // todo: make this an enum
    public string Type { get; set; } = "Person";

    public Uri Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string PreferredUsername { get; set; } = string.Empty;
    public string? Summary { get; set; }
    [JsonPropertyName("published")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonPropertyName("inbox")]
    public Uri InboxUrl { get; set; } = default!;
    [JsonPropertyName("followers")]
    public Uri FollowersUrl { get; set; } = default!;

    public PublicKey PublicKey { get; set; } = default!;

    internal void AddDomain(Uri domain)
    {
        if (!Id.IsAbsoluteUri)
        {
            Id = new Uri(domain, Id);
        }

        if (!InboxUrl.IsAbsoluteUri)
        {
            InboxUrl = new Uri(domain, InboxUrl);
        }

        if (!FollowersUrl.IsAbsoluteUri)
        {
            FollowersUrl = new Uri(domain, FollowersUrl);
        }

        PublicKey.AddDomain(domain);
    }
}
