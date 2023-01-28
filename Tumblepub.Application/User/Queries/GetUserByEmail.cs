using Mediator;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Aggregates.User?>;

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, Aggregates.User?>
{
    private readonly IRepository<Aggregates.User, Guid> _userRepository;

    public GetUserByEmailQueryHandler(IRepository<Aggregates.User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async ValueTask<Aggregates.User?> Handle(GetUserByEmailQuery query, CancellationToken token = default)
    {
        return await _userRepository.FirstOrDefaultAsync(u => u.Email == query.Email, token);
    }
}