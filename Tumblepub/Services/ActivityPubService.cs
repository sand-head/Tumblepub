using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.Models;
using Tumblepub.Database.Repositories;

namespace Tumblepub.Services;

public class ActivityPubService : IActivityPubService
{
    private readonly IUserRepository _userRepository;

    public ActivityPubService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Actor> GetUser(Guid id, CancellationToken token = default)
    {
        var user = await _userRepository.GetByIdAsync(id, token);
        throw new NotImplementedException();
    }
}
