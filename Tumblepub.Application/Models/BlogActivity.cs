namespace Tumblepub.Application.Models;

public enum ObjectType
{
    Blog,
    Post
}

public class BlogActivity : Aggregate
{
    public Guid BlogId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }

    public ObjectType? ObjectType { get; set; }
    public Guid? ObjectId { get; set; }

    public ObjectType? TargetType { get; set; }
    public IEnumerable<Guid>? TargetIds { get; set; }

    public ObjectType? OriginType { get; set; }
    public Guid? OriginId { get; set; }
}