using Tumblepub.Application.Aggregates;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(Blog),
    IgnoreProperties = new[] { nameof(Blog.Id), nameof(Blog.UserId), nameof(Blog.PrivateKey) })]
public class BlogTypeExtensions
{
}
