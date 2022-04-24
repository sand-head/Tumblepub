using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Tumblepub.Infrastructure;

namespace Tumblepub;

public record AuthResult(string AccessToken, string RefreshToken);

public class Mutation
{
    public async Task<AuthResult> Register(
        string email,
        string password,
        string name,
        [Service] IUserRepository userRepository,
        [Service] IBlogRepository blogRepository,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        var user = await userRepository.CreateAsync(email, password);
        await blogRepository.CreateAsync(user.Id, name);

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] IUserRepository userRepository,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        if (!await userRepository.ValidateCredentialsAsync(email, password))
        {
            throw new Exception("bad");
        }

        var user = await userRepository.GetByEmailAsync(email);
        var result = authenticationManager.GenerateTokens(user!);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    public async Task<Blog> CreateBlog(ClaimsPrincipal claimsPrincipal, string name,
        [Service] IUserRepository userRepository,
        [Service] IBlogRepository blogRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("bad");
        }

        var blog = await blogRepository.CreateAsync(user.Id, name);
        return blog;
    }

    [Authorize]
    public async Task<Post> CreatePost(ClaimsPrincipal claimsPrincipal,
        string blogName, string content, string[] tags,
        [Service] IBlogRepository blogRepository,
        [Service] IPostRepository postRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var blog = await blogRepository.GetByNameAsync(blogName, null);
        if (blog == null || blog.UserId != userId)
        {
            throw new Exception("bad");
        }

        var post = await postRepository.CreateMarkdownPost(blog.Id, content, tags);
        return post;
    }
}
