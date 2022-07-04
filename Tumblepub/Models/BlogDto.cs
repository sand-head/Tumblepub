using System.Text.Json;

namespace Tumblepub.Models;

[GraphQLName("Blog")]
public class BlogDto
{
    [GraphQLIgnore]
    public Guid Id { get; set; }
    [GraphQLIgnore]
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonDocument? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    [GraphQLIgnore]
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}