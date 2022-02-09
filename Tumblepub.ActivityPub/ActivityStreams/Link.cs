﻿using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;
using Tumblepub.ActivityPub.Extensions;

namespace Tumblepub.ActivityPub.ActivityStreams;

[JsonConverter(typeof(LinkConverter))]
public record Link(Uri Href) : ActivityStreamsValue("Link")
{
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<string>? Rel { get; init; }
    public string? MediaType { get; init; }
    public string? Name { get; init; }
    public string? Hreflang { get; init; }
    public int? Height { get; init; }
    public int? Width { get; init; }
    public Uri? Preview { get; init; }
}