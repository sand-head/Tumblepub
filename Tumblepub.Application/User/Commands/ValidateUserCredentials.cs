using Isopoh.Cryptography.Argon2;
using Mediator;
using Tumblepub.Application.User.Queries;

namespace Tumblepub.Application.User.Commands;

public record ValidateUserCredentialsCommand(string Email, string Password) : ICommand<bool>;

public class ValidateUserCredentials : ICommandHandler<ValidateUserCredentialsCommand, bool>
{
    private readonly IMediator _mediator;
    
    public ValidateUserCredentials(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async ValueTask<bool> Handle(ValidateUserCredentialsCommand command, CancellationToken token = default)
    {
        var query = new GetUserByEmailQuery(command.Email);
        var user = await _mediator.Send(query, token);

        return user != null && Argon2.Verify(user.PasswordHash, command.Password);
    }
}