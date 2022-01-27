using HotChocolate;

namespace Tumblepub.Database.Models;

public class UserDto
{
    [GraphQLIgnore]
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    [GraphQLIgnore]
    public List<Guid> Blogs { get; set; } = new();
}
