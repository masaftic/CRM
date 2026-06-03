namespace SalesModule.Infrastructure.Data.ReadModels;

public class SalesContact
{
    public Guid ContactId { get; set; }
    public required string Name { get; set; }
    public required string CompanyName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset SyncedAt { get; set; } = DateTimeOffset.UtcNow;
}
