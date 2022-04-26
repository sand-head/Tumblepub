namespace Tumblepub.Application.Interfaces;

public interface IQuery<TResult>
{
}

public interface IQueryHandler<in TQuery, out TResult>
    where TQuery : IQuery<TResult>
{
    TResult Handle(TQuery query);
}