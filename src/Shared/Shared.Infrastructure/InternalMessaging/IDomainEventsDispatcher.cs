using BuildingBlocks.Domain;

namespace Shared.Infrastructure.InternalMessaging;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return DispatchAsync([domainEvent], cancellationToken);
    }
}
