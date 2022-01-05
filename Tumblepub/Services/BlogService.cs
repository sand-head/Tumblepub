using Tumblepub.Database.Projections;

namespace Tumblepub.Services;

public interface IBlogService
{
    Task<Blog> CreateAsync(Guid userId, string name);
}

public class BlogService
{
}
