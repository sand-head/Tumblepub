using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using HotChocolate.Subscriptions;
using Mediator;
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
        [Service] IMediator mediator,
        [Service] JwtAuthenticationManager authenticationManager,
        CancellationToken token)
    {
        var command = new CreateUserCommand(email, name, password);
        var user = await mediator.Send(command, token);

        var result = authenticationManager.GenerateTokens(user);
        return new AuthResult(result.AccessToken, result.RefreshToken.Token);
    }

    public async Task<AuthResult> Login(string email, string password,
        [Service] IMediator mediator,
        [Service] JwtAuthenticationManager authenticationManager,
        CancellationToken token)
    {
        var validateCommand = new ValidateUserCredentialsCommand(email, password);
        if (!await mediator.Send(validateCommand, token))
        {
            throw new Exception("bad");
        }

        var query = new GetUserByEmailQuery(email);
        var user = await mediator.Send(query, token);
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
        [Service] IMediator mediator,
        CancellationToken token)
    {
        var userId = claimsPrincipal.GetUserId();
        var query = new GetUserByIdQuery(userId);
        var user = await mediator.Send(query, token);
        
        if (user == null)
        {
            throw new Exception("bad");
        }

        var command = new CreateBlogCommand(userId, name);
        var blog = await mediator.Send(command, token);
        return mapper.Map<BlogDto>(blog);
    }

    [Authorize]
    public async Task<PostDto> CreatePost(ClaimsPrincipal claimsPrincipal,
        string blogName, string content, List<string> tags,
        [Service] ILogger<Mutation> logger,
        [Service] IMapper mapper,
        [Service] IMediator mediator,
        [Service] ITopicEventSender sender,
        CancellationToken token)
    {
        var userId = claimsPrincipal.GetUserId();

        logger.LogInformation("Creating post for blog {BlogName}", blogName);
        var query = new GetBlogByNameQuery(blogName);
        var blog = await mediator.Send(query, token);
        if (blog == null || blog.UserId != userId)
        {
            throw new Exception("bad");
        }

        var command = new CreatePostCommand(blog.Id, content, tags);
        var post = await mediator.Send(command, token);
        var postDto = mapper.Map<PostDto>(post);

        await sender.SendAsync($"{blogName}_{nameof(Subscription.PostCreated)}", postDto, token);
        return postDto;
    }
}
