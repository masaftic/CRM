namespace SalesModule.Contracts.Deals.Events;

public record DealWonIntegrationEvent(
    string DealId,
    Guid ContactId,
    string Name,
    decimal Amount,
    string Currency,
    DateTimeOffset WonAt);
