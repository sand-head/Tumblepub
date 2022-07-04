using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.User.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<Aggregates.User?>;

internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Aggregates.User?>
{
    private readonly IRepository<Aggregates.User, Guid> _userRepository;

    public GetUserByIdQueryHandler(IRepository<Aggregates.User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Aggregates.User?> Handle(GetUserByIdQuery query, CancellationToken token = default)
    {
        return await _userRepository.GetByIdAsync(query.Id, token);
    }
}