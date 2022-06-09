using System.Diagnostics;
using Isopoh.Cryptography.Argon2;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Extensions;

public static class UserRepositoryExtensions
{
    public static async Task<bool> ValidateCredentialsAsync(this IReadOnlyRepository<Models.User, UserId> repository, string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByEmailAsync(email, cancellationToken);

        return user != null && Argon2.Verify(user.PasswordHash, password);
    }

    public static async Task<Models.User?> GetByEmailAsync(this IReadOnlyRepository<Models.User, UserId> repository, string email, CancellationToken cancellationToken = default)
    {
        return await repository.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}