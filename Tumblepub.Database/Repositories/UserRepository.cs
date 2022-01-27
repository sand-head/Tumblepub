using Marten;
using Microsoft.Extensions.Logging;
using Sodium;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(string email, string password);
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDocumentSession _session;

    public UserRepository(ILogger<UserRepository> logger, IDocumentSession session)
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
        _logger.LogInformation("Created new user {Id}", userCreated.UserId);

        var user = await _session.LoadAsync<User>(userCreated.UserId);
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
