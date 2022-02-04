using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.Models;

public class PublicKey
{
    public string Id { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string PublicKeyPem { get; set; } = string.Empty;
}

public class Actor
{
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new();
    // todo: make this an enum
    public string Type { get; set; } = "Person";

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PreferredUsername { get; set; } = string.Empty;
    public string? Summary { get; set; }
    [JsonPropertyName("published")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonPropertyName("inbox")]
    public string InboxUrl { get; set; } = default!;
    [JsonPropertyName("followers")]
    public string FollowersUrl { get; set; } = default!;

    public PublicKey PublicKey { get; set; } = default!;
}
