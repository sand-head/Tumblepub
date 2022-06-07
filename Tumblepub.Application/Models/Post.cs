using StronglyTypedIds;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct PostId { }

public class Post : Aggregate<PostId>
{
    public BlogId BlogId { get; set; }
    public PostContent Content { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public abstract record PostContent()
{
    public record External(Uri ExternalId) : PostContent();

    public record Deleted() : PostContent();

    public abstract record Internal() : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }

    public record Markdown(string Content) : Internal();
}