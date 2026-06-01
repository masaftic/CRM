namespace SalesModule.Contracts.Deals.Events;

public record DealCreatedIntegrationEvent(
    string DealId,
    string Name,
    decimal Amount,
    string PipelineId);
