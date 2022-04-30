namespace Tumblepub.Application.Interfaces;

public interface ICommand<TResult>
{
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken token = default);
}