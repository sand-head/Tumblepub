using Tumblepub.Infrastructure;

namespace Tumblepub.GraphQL;

public record AuthResult(string AccessToken, string RefreshToken);

public class Mutation
{
    public async Task<AuthResult> Register(
        string email,
        string password,
        string name,
        [Service] IUserService userService,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        var user = await userService.CreateAsync(email, password);

        // todo: create blog

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public async Task RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
