using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Commands;

public record CreateBlogCommand(Guid UserId, string Name) : ICommand<Aggregates.Blog>;

internal class CreateBlogCommandHandler : ICommandHandler<CreateBlogCommand, Aggregates.Blog>
{
    private readonly IRepository<Aggregates.Blog, Guid> _blogRepository;

    public CreateBlogCommandHandler(IRepository<Aggregates.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }
    
    public async Task<Aggregates.Blog> Handle(CreateBlogCommand command, CancellationToken token = default)
    {
        var (userId, name) = command;
        
        var blog = new Aggregates.Blog(userId, name);
        await _blogRepository.CreateAsync(blog, token);
        await _blogRepository.SaveChangesAsync(token);

        return blog;
    }
}