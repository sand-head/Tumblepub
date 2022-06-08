using System.Linq.Expressions;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IRepository : IDisposable
{
    Task SaveChangesAsync(CancellationToken token = default);
}

public interface IQueryableRepository<out TAggregate, in TId> : IRepository
    where TAggregate : class, IAggregate<TId>
    where TId : struct
{
    IQueryable<TAggregate> Query();
}

public interface IReadOnlyRepository<TAggregate, in TId> : IRepository
    where TAggregate : class, IAggregate<TId>
    where TId : struct
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="whereCondition"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<TAggregate>> GetAllAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default);
    /// <summary>
    /// Gets a(n) <see cref="TAggregate"/> by its <see cref="IAggregate.Id"/>.
    /// </summary>
    /// <param name="id">The <see cref="IAggregate.Id"/>.</param>
    /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="TAggregate"/>, or null if none exists.</returns>
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken token = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="whereCondition"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<TAggregate?> FirstOrDefaultAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default);
}


public interface IRepository<TAggregate, in TId> : IReadOnlyRepository<TAggregate, TId>
    where TAggregate : class, ISelfAggregate<TId>
    where TId : struct
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregate"></param>
    /// <param name="token"></param>
    /// <returns>The current <see cref="IAggregate.Version"/>.</returns>
    Task<long> CreateAsync(TAggregate aggregate, CancellationToken token = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregate"></param>
    /// <param name="token"></param>
    /// <returns>The current <see cref="IAggregate.Version"/>.</returns>
    Task<long> UpdateAsync(TAggregate aggregate, CancellationToken token = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregate"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<long> DeleteAsync(TAggregate aggregate, CancellationToken token = default);
}