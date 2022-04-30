using System.Linq.Expressions;

namespace Tumblepub.Infrastructure.Infrastructure;

public class MartenAsyncQueryProvider : IAsyncQueryProvider
{
    public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}