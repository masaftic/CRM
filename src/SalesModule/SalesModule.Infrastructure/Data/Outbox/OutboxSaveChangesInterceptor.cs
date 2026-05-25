using System.Text.Json;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SalesModule.Infrastructure.Data.Outbox;

public class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public OutboxSaveChangesInterceptor(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxEntries = context.ChangeTracker.Entries<IAggregate>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        foreach (var entry in outboxEntries)
        {
            var outboxTableEntry = new OutboxMessage
            {
                Type = entry.GetType(),
                JsonData = JsonSerializer.Serialize(entry, entry.GetType(), _jsonSerializerOptions)
            };

            context.Set<OutboxMessage>().Add(outboxTableEntry);
        }

        foreach (var entry in outboxEntries)
        {
            entry.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

}
