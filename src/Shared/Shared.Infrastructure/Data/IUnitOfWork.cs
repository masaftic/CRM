using BuildingBlocks.Domain;

namespace Shared.Infrastructure.Data;

public interface IUnitOfWork
{
    IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() where TAggregate : IAggregate;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
