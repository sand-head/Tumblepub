using System.Linq.Expressions;
using Marten.Internal.Sessions;
using Marten.Linq;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Infrastructure;

public class MartenAsyncQueryable<TAggregate> : IAsyncQueryable<TAggregate>, IAsyncQueryProvider
    where TAggregate : class, IAggregate
{
    private readonly IMartenQueryable<TAggregate> _martenQueryable;
    
    public MartenAsyncQueryable(IMartenQueryable<TAggregate> martenQueryable)
    {
        _martenQueryable = martenQueryable;
        
        ElementType = martenQueryable.ElementType;
        Expression = martenQueryable.Expression;
        Provider = this;
    }
    
    public Type ElementType { get; }
    public Expression Expression { get; }
    public IAsyncQueryProvider Provider { get; }

    public IAsyncEnumerator<TAggregate> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        return _martenQueryable.ToAsyncEnumerable(cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}