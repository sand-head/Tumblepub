using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(string email, string password);
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}