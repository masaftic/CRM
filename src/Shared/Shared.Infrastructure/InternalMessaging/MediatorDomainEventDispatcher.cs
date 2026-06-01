using System.Collections.Concurrent;
using System.Reflection;
using BuildingBlocks.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.InternalMessaging;

public class MediatorDomainEventDispatcher(IPublisher publisher) : IDomainEventsDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish((object)domainEvent, cancellationToken);
        }
    }
}
