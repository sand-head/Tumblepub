using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
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
using Tumblepub.Models;

namespace Tumblepub;

public class Mutation
{
    public async Task<AuthResult> Register(
        string email,
        string password,
        string name,
        [Service] ICommandHandler<CreateUserCommand, User> createUserCommandHandler,
        [Service] JwtAuthenticationManager authenticationManager,
        CancellationToken token)
    {
        var command = new CreateUserCommand(email, name, password);
        var user = await createUserCommandHandler.Handle(command, token);

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] ICommandHandler<ValidateUserCredentialsCommand, bool> validateCommandHandler,
        [Service] IQueryHandler<GetUserByEmailQuery, User?> getByEmailQueryHandler,
        [Service] JwtAuthenticationManager authenticationManager,
        CancellationToken token)
    {
        var validateCommand = new ValidateUserCredentialsCommand(email, password);
        if (!await validateCommandHandler.Handle(validateCommand, token))
        {
            throw new Exception("bad");
        }

        var query = new GetUserByEmailQuery(email);
        var user = await getByEmailQueryHandler.Handle(query, token);
        var result = authenticationManager.GenerateTokens(user!);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    public async Task<BlogDto> CreateBlog(ClaimsPrincipal claimsPrincipal, string name,
        [Service] IMapper mapper,
        [Service] IQueryHandler<GetUserByIdQuery, User?> getByIdQueryHandler,
        [Service] ICommandHandler<CreateBlogCommand, Blog> createBlogCommandHandler,
        CancellationToken token)
    {
        var userId = claimsPrincipal.GetUserId();
        var query = new GetUserByIdQuery(userId);
        var user = await getByIdQueryHandler.Handle(query, token);
        
        if (user == null)
        {
            throw new Exception("bad");
        }

        var command = new CreateBlogCommand(userId, name);
        var blog = await createBlogCommandHandler.Handle(command, token);
        return mapper.Map<BlogDto>(blog);
    }

    [Authorize]
    public async Task<PostDto> CreatePost(ClaimsPrincipal claimsPrincipal,
        string blogName, string content, List<string> tags,
        [Service] ILogger<Mutation> logger,
        [Service] IMapper mapper,
        [Service] IQueryHandler<GetBlogByNameQuery, Blog?> getBlogByNameQueryHandler,
        [Service] ICommandHandler<CreatePostCommand, Post> createPostCommandHandler,
        CancellationToken token)
    {
        var userId = claimsPrincipal.GetUserId();

        logger.LogInformation("Creating post for blog {BlogName}", blogName);
        var query = new GetBlogByNameQuery(blogName);
        var blog = await getBlogByNameQueryHandler.Handle(query, token);
        if (blog == null || blog.UserId != userId)
        {
            throw new Exception("bad");
        }

        var command = new CreatePostCommand(blog.Id, content, tags);
        var post = await createPostCommandHandler.Handle(command, token);
        return mapper.Map<PostDto>(post);
    }
}
