using Marten;
using Sodium;
using Tumblepub.Database.Events;
using Tumblepub.Database.Projections;

namespace Tumblepub.Infrastructure
{
    public interface IUserService
    {
        Task<User> CreateAsync(string email, string password);
        Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
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

            return await _session.Events.AggregateStreamAsync<User>(userCreated.UserId);
        }

        public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _session.Query<User>()
                .Where(u => u.Email == email)
                .AnyAsync(cancellationToken);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _session.Query<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync(cancellationToken);

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
    }
}
