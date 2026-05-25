namespace SalesModule.Infrastructure.Data.Outbox;

public class OutboxMessage
{
    public int Id { get; set; }
    public required Type Type { get; set; }
    public required string JsonData { get; set; }
    public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
}

