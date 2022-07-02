using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Post.Commands;

public record CreatePostCommand(Guid BlogId, string Content, List<string> Tags) : ICommand<Models.Post>;

internal class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Models.Post>
{
    private readonly IRepository<Models.Post, Guid> _postRepository;

    public CreatePostCommandHandler(IRepository<Models.Post, Guid> postRepository)
    {
        _postRepository = postRepository;
    }
    
    public async Task<Models.Post> Handle(CreatePostCommand command, CancellationToken token = default)
    {
        var (blogId, content, tags) = command;
        
        var postContent = new PostContent.Markdown(content)
        {
            Tags = tags
        };
        var post = new Models.Post(blogId, postContent);
        
        await _postRepository.CreateAsync(post, token);
        await _postRepository.SaveChangesAsync(token);

        return post;
    }
}