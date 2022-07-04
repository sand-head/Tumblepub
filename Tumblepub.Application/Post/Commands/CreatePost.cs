using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Post.Commands;

public record CreatePostCommand(Guid BlogId, string Content, List<string> Tags) : ICommand<Aggregates.Post>;

internal class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Aggregates.Post>
{
    private readonly IRepository<Aggregates.Post, Guid> _postRepository;

    public CreatePostCommandHandler(IRepository<Aggregates.Post, Guid> postRepository)
    {
        _postRepository = postRepository;
    }
    
    public async Task<Aggregates.Post> Handle(CreatePostCommand command, CancellationToken token = default)
    {
        var (blogId, content, tags) = command;
        
        var postContent = new PostContent.Markdown(content)
        {
            Tags = tags
        };
        var post = new Aggregates.Post(blogId, postContent);
        
        await _postRepository.CreateAsync(post, token);
        await _postRepository.SaveChangesAsync(token);

        return post;
    }
}