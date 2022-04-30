using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Models.User?>;

internal class GetUserByEmail : IQueryHandler<GetUserByEmailQuery, Models.User?>
{
    private readonly IRepository<Models.User> _userRepository;

    public GetUserByEmail(IRepository<Models.User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Models.User?> Handle(GetUserByEmailQuery query, CancellationToken token = default)
    {
        return await _userRepository.Query()
            .Where(u => u.Email == query.Email)
            .FirstOrDefaultAsync(token);
    }
}