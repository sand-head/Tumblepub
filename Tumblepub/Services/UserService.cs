using Marten;
using Sodium;
using Tumblepub.Events;
using Tumblepub.Projections;

namespace Tumblepub.Services;

public interface IUserService
{
    Task<User> CreateAsync(string email, string password);
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IDocumentSession _session;

    public UserService(ILogger<UserService> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<User> CreateAsync(string email, string password)
    {
        var hashedPassword = PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Medium).TrimEnd('\0');
        var userCreated = new UserCreated(Guid.NewGuid(), email, hashedPassword, DateTimeOffset.UtcNow);

        _session.Events.StartStream<User>(userCreated.UserId, userCreated);
        await _session.SaveChangesAsync();
        _logger.LogDebug("Created new user {Id}", userCreated.UserId);

        var user = await _session.Events.AggregateStreamAsync<User>(userCreated.UserId);
        return user!;
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await GetByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            return false;
        }

        if (!PasswordHash.ArgonHashStringVerify(user.PasswordHash, password))
        {
            return false;
        }

        return true;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _session.Query<User>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.Query<User>()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
