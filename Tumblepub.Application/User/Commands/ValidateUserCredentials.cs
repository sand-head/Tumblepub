using Isopoh.Cryptography.Argon2;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.User.Queries;

namespace Tumblepub.Application.User.Commands;

public record ValidateUserCredentialsCommand(string Email, string Password) : ICommand<bool>;

internal class ValidateUserCredentials : ICommandHandler<ValidateUserCredentialsCommand, bool>
{
    private readonly IQueryHandler<GetUserByEmailQuery, Aggregates.User?> _getUserByEmailQuery;

    public ValidateUserCredentials(IQueryHandler<GetUserByEmailQuery, Aggregates.User?> getUserByEmailQuery)
    {
        _getUserByEmailQuery = getUserByEmailQuery;
    }

    public async Task<bool> Handle(ValidateUserCredentialsCommand command, CancellationToken token = default)
    {
        var query = new GetUserByEmailQuery(command.Email);
        var user = await _getUserByEmailQuery.Handle(query, token);

        return user != null && Argon2.Verify(user.PasswordHash, command.Password);
    }
}