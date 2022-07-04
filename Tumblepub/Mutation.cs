using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Commands;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Post.Commands;
using Tumblepub.Application.User.Commands;
using Tumblepub.Application.User.Queries;
using Tumblepub.Extensions;
using Tumblepub.Infrastructure;

namespace Tumblepub;

public record AuthResult(string AccessToken, string RefreshToken);

public class Mutation
{
    public async Task<AuthResult> Register(
        string email,
        string password,
        string name,
        [Service] ICommandHandler<CreateUserCommand, User> createUserCommandHandler,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        var command = new CreateUserCommand(email, name, password);
        var user = await createUserCommandHandler.Handle(command);

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] ICommandHandler<ValidateUserCredentialsCommand, bool> validateCommandHandler,
        [Service] IQueryHandler<GetUserByEmailQuery, User?> getByEmailQueryHandler,
        [Service] JwtAuthenticationManager authenticationManager)
    {
        var validateCommand = new ValidateUserCredentialsCommand(email, password);
        if (!await validateCommandHandler.Handle(validateCommand))
        {
            throw new Exception("bad");
        }

        var query = new GetUserByEmailQuery(email);
        var user = await getByEmailQueryHandler.Handle(query);
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
        [Service] ICommandHandler<CreateBlogCommand, Blog> createBlogCommandHandler)
    {
        var userId = claimsPrincipal.GetUserId();
        var user = await userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new Exception("bad");
        }

        var command = new CreateBlogCommand(userId, name);
        return await createBlogCommandHandler.Handle(command);
    }

    [Authorize]
    public async Task<Post> CreatePost(ClaimsPrincipal claimsPrincipal,
        string blogName, string content, List<string> tags,
        [Service] IQueryHandler<GetBlogByNameQuery, Blog?> getBlogByNameQueryHandler,
        [Service] ICommandHandler<CreatePostCommand, Post> createPostCommandHandler)
    {
        var userId = claimsPrincipal.GetUserId();

        var query = new GetBlogByNameQuery(blogName);
        var blog = await getBlogByNameQueryHandler.Handle(query);
        if (blog == null || blog.UserId != userId)
        {
            throw new Exception("bad");
        }

        var command = new CreatePostCommand(blog.Id, content, tags);
        return await createPostCommandHandler.Handle(command);
    }
}
