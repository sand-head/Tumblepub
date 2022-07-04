namespace Tumblepub.Application.Aggregates;

public abstract record ObjectType()
{
    public record Blog(Guid BlogId) : ObjectType();
    public record Post(Guid BlogId) : ObjectType();
}

public class BlogActivity : ReadOnlyAggregate<Guid>
{
    public BlogActivity() { }
    
    public BlogActivity(Guid id)
    {
        Id = id;
    }
    
    public Guid BlogId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }

    public ObjectType? ObjectType { get; set; }
    public Guid? ObjectId { get; set; }

    public ObjectType? TargetType { get; set; }
    public ObjectType? OriginType { get; set; }
}