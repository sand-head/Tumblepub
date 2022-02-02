using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumblepub.ActivityPub.Models;

namespace Tumblepub.ActivityPub;

public interface IActivityPubService
{
    Task<Actor> GetUser(Guid id);
}
