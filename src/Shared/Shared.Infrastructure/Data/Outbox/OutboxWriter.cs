using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Data.Outbox;

public sealed class OutboxWriter<TDbContext>(
    TDbContext dbContext,
    JsonSerializerOptions jsonSerializerOptions) : IOutboxWriter
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
