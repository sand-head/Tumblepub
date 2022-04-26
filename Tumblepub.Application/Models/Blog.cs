using System.Text.Json;

namespace Tumblepub.Application.Models;

public class Blog : Aggregate
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonDocument? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}