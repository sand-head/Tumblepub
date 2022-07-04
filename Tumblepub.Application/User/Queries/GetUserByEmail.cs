using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Aggregates.User?>;

internal class GetUserByEmail : IQueryHandler<GetUserByEmailQuery, Aggregates.User?>
{
    private readonly IRepository<Aggregates.User, Guid> _userRepository;

    public GetUserByEmail(IRepository<Aggregates.User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Aggregates.User?> Handle(GetUserByEmailQuery query, CancellationToken token = default)
    {
        return await _userRepository.FirstOrDefaultAsync(u => u.Email == query.Email, token);
    }
}