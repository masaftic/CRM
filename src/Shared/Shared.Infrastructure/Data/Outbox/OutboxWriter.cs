using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Data.Outbox;

public class OutboxWriter<TDbContext, TModule>(
    TDbContext dbContext,
    JsonSerializerOptions jsonSerializerOptions) : IOutboxWriter<TModule>
    where TDbContext : DbContext, IOutboxDbContext
{
    public void Write(OutboxMessage message)
    {
        dbContext.OutboxMessages.Add(message);
    }

    public void Write(object message)
    {
        Write(OutboxMessage.From(message, jsonSerializerOptions));
    }
}
