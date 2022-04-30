using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IRepository<TAggregate>
    where TAggregate : class, IAggregate
{
    /// <summary>
    /// Gets a(n) <see cref="TAggregate"/> by its <see cref="IAggregate.Id"/>.
    /// </summary>
    /// <param name="id">The <see cref="IAggregate.Id"/>.</param>
    /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="TAggregate"/>, or null if none exists.</returns>
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken token = default);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IAsyncQueryable<TAggregate> Query();
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
}