using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Commands;

public record CreateUserCommand(string Email, string Username, string Password) : ICommand<Aggregates.User>;

internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Aggregates.User>
{
    private readonly IRepository<Aggregates.User, Guid> _userRepository;
    private readonly IRepository<Aggregates.Blog, Guid> _blogRepository;

    public CreateUserCommandHandler(IRepository<Aggregates.User, Guid> userRepository, IRepository<Aggregates.Blog, Guid> blogRepository)
    {
        _userRepository = userRepository;
        _blogRepository = blogRepository;
    }
    
    public async Task<Aggregates.User> Handle(CreateUserCommand command, CancellationToken token = default)
    {
        var (email, name, password) = command;
        
        // todo: probably wrap these in a transaction
        var user = new Aggregates.User(email, password);
        await _userRepository.CreateAsync(user, token);
        await _userRepository.SaveChangesAsync(token); // not sure how much I like this...
        
        // all users need at least one blog
        var blog = new Aggregates.Blog(user.Id, name);
        await _blogRepository.CreateAsync(blog, token);
        await _blogRepository.SaveChangesAsync(token);

        return user;
    }
}