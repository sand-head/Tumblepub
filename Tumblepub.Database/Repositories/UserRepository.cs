using Isopoh.Cryptography.Argon2;
using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Events;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal class UserRepository : IUserRepository
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
        var hashedPassword = Argon2.Hash(password).TrimEnd('\0');
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

        if (!Argon2.Verify(user.PasswordHash, password))
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
