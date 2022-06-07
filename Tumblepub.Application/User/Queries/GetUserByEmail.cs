using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Models.User?>;

internal class GetUserByEmail : IQueryHandler<GetUserByEmailQuery, Models.User?>
{
    private readonly IRepository<Models.User, UserId> _userRepository;

    public GetUserByEmail(IRepository<Models.User, UserId> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Models.User?> Handle(GetUserByEmailQuery query, CancellationToken token = default)
    {
        return await _userRepository.FirstOrDefaultAsync(u => u.Email == query.Email, token);
    }
}