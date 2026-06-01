using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Data.Outbox;

public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
}
