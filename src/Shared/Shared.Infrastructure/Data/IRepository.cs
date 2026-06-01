namespace Shared.Infrastructure.Data;

public interface IRepository<TAggregate, TKey>
{
    Task<TAggregate?> TryFindAsync(TKey id, CancellationToken cancellationToken = default);
    void Add(TAggregate aggregate);
    void Delete(TAggregate aggregate);

    public async Task Delete(TKey id, CancellationToken cancellationToken = default)
    {
        var aggregate = await TryFindAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Aggregate with id {id} not found.");
        Delete(aggregate);
    }
}
