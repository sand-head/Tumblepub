using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Commands;

public record CreateBlogCommand(Guid UserId, string Name) : ICommand<Models.Blog>;

internal class CreateBlogCommandHandler : ICommandHandler<CreateBlogCommand, Models.Blog>
{
    private readonly IRepository<Models.Blog, Guid> _blogRepository;

    public CreateBlogCommandHandler(IRepository<Models.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }
    
    public async Task<Models.Blog> Handle(CreateBlogCommand command, CancellationToken token = default)
    {
        var (userId, name) = command;
        
        var blog = new Models.Blog(userId, name);
        await _blogRepository.CreateAsync(blog, token);
        await _blogRepository.SaveChangesAsync(token);

        return blog;
    }
}