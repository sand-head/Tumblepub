using Marten;
using Tumblepub.Projections;

namespace Tumblepub.Services;

public interface IUserDtoService
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class UserDtoService : IUserDtoService
{
    private readonly ILogger<UserService> _logger;
    private readonly IDocumentSession _session;

    public UserDtoService(ILogger<UserService> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.Query<UserDto>()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
