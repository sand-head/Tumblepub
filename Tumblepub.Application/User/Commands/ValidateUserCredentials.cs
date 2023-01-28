using Isopoh.Cryptography.Argon2;
using MediatR;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.User.Queries;

namespace Tumblepub.Application.User.Commands;

public record ValidateUserCredentialsCommand(string Email, string Password) : IRequest<bool>;

internal class ValidateUserCredentials : IRequestHandler<ValidateUserCredentialsCommand, bool>
{
    private readonly IRequestHandler<GetUserByEmailQuery, Aggregates.User?> _getUserByEmailQuery;

    public ValidateUserCredentials(IRequestHandler<GetUserByEmailQuery, Aggregates.User?> getUserByEmailQuery)
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