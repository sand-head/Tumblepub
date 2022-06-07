using System.Text.Json;
using StronglyTypedIds;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct BlogId { }

public class Blog : Aggregate<BlogId>
{
    public UserId? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonDocument? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}