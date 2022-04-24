using System.Linq.Expressions;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IBlogActivityRepository
{
    Task<BlogActivity?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<BlogActivity>> GetByBlogIdAsync(Guid blogId, int page = 0, CancellationToken token = default);
    Task<int> CountAsync(Expression<Func<BlogActivity, bool>>? condition = null, CancellationToken token = default);
}