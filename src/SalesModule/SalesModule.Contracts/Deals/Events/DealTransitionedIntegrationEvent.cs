namespace SalesModule.Contracts.Deals.Events;

public record DealTransitionedIntegrationEvent(
    string DealId,
    string FromStageId,
    string ToStageId,
    DateTimeOffset TransitionedAt);
