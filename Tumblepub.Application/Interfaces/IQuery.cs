namespace Tumblepub.Application.Interfaces;

public interface IQuery<TResult>
{
}

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken token = default);
}