using Tumblepub.GraphQL.Models;

namespace Tumblepub.GraphQL;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    public User GetCurrentUser()
    {
        throw new NotImplementedException();
    }
}
