using System.Text.Json;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Infrastructure.InternalMessaging;

namespace Shared.Infrastructure.Data;

public class DomainEventsInterceptor(IDomainEventsDispatcher dispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entities = context.ChangeTracker.Entries<IAggregate>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entry in entities)
        {
            entry.ClearDomainEvents();
        }

        await dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
