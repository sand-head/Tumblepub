using MediatR;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Aggregates.User?>;

internal class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Aggregates.User?>
{
    private readonly IRepository<Aggregates.User, Guid> _userRepository;

    public GetUserByEmailQueryHandler(IRepository<Aggregates.User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Aggregates.User?> Handle(GetUserByEmailQuery query, CancellationToken token = default)
    {
        return await _userRepository.FirstOrDefaultAsync(u => u.Email == query.Email, token);
    }
}