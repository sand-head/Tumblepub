using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tumblepub.ActivityPub.Models;

public class PublicKey
{
    public string Id { get; set; }
    public string Owner { get; set; }
    public string PublicKeyPem { get; set; }
}

public class Actor
{
    [JsonPropertyName("@context")]
    public JsonElement Context { get; set; }
    [JsonPropertyName("type")]
    public string Kind { get; set; }

    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PreferredUsername { get; set; } = string.Empty;
    public string? Summary { get; set; }
    [JsonPropertyName("published")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonPropertyName("inbox")]
    public string InboxUrl { get; set; }
    [JsonPropertyName("outbox")]
    public string OutboxUrl { get; set; }

    public PublicKey PublicKey { get; set; } = default!;
}
