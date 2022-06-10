using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using System.Transactions;
using Tumblepub.Application.Extensions;
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
        [Service] IRepository<User, Guid> userRepository,
        [Service] IRepository<Blog, Guid> blogRepository,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        // todo: probably wrap these in a transaction
        var user = new User(email, password);
        await userRepository.CreateAsync(user);
        await userRepository.SaveChangesAsync(); // not sure how much I like this...
        
        var blog = new Blog(user.Id, name);
        await blogRepository.CreateAsync(blog);
        await blogRepository.SaveChangesAsync();

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] IReadOnlyRepository<User, Guid> userRepository,
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
        [Service] IReadOnlyRepository<User, Guid> userRepository,
        [Service] IRepository<Blog, Guid> blogRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("bad");
        }

        var blog = new Blog(user.Id, name);
        await blogRepository.CreateAsync(blog);
        await blogRepository.SaveChangesAsync();
        
        return blog;
    }

    [Authorize]
    public async Task<Post> CreatePost(ClaimsPrincipal claimsPrincipal,
        string blogName, string content, List<string> tags,
        [Service] IReadOnlyRepository<Blog, Guid> blogRepository,
        [Service] IRepository<Post, Guid> postRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var blog = await blogRepository.GetByNameAsync(blogName, null);
        if (blog == null || blog.UserId != userId)
        {
            throw new Exception("bad");
        }

        var postContent = new PostContent.Markdown(content)
        {
            Tags = tags
        };
        var post = new Post(blog.Id, postContent);
        await postRepository.CreateAsync(post);
        await postRepository.SaveChangesAsync();
        
        return post;
    }
}
