using HotChocolate.Resolvers;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(User),
    IgnoreProperties = new[] { nameof(User.Id), nameof(User.PasswordHash) })]
public class UserTypeExtensions
{
}
