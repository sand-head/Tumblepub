using Tumblepub.Infrastructure;
using Tumblepub.Projections;
using Tumblepub.Services;

namespace Tumblepub;

public record AuthResult(string AccessToken, string RefreshToken);

public class Mutation
{
    public async Task<AuthResult> Register(
        string email,
        string password,
        string name,
        [Service] IUserService userService,
        [Service] IBlogService blogService,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        var user = await userService.CreateAsync(email, password);
        await blogService.CreateAsync(user.Id, name);

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] IUserService userService,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        if (!await userService.ValidateCredentialsAsync(email, password))
        {
            throw new Exception("bad");
        }

        var user = await userService.GetByEmailAsync(email);
        var result = authenticationManager.GenerateTokens(user!);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Blog> CreateBlog()
    {
        throw new NotImplementedException();
    }
}
