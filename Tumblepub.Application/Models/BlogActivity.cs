using StronglyTypedIds;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct BlogActivityId { }

public abstract record ObjectType()
{
    public record Blog(BlogId BlogId) : ObjectType();
    public record Post(PostId BlogId) : ObjectType();
}

public class BlogActivity : ReadOnlyAggregate<BlogActivityId>
{
    public BlogActivity() { }
    
    public BlogActivity(BlogActivityId id)
    {
        Id = id;
    }
    
    public BlogId BlogId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }

    public ObjectType? ObjectType { get; set; }
    public Guid? ObjectId { get; set; }

    public ObjectType? TargetType { get; set; }
    public ObjectType? OriginType { get; set; }
}