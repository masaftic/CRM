using BuildingBlocks.Domain;

namespace SalesModule.Domain.Events;

public record DealCreated(DealId Id, Guid ContactId, string Name, decimal Amount, PipelineId PipelineId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealUpdated(DealId Id, Guid ContactId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealDeleted(DealId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealWon(DealId Id, Guid ContactId, string Name, decimal Amount, string Currency) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealLost(DealId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealReopened(DealId Id) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record DealTransitioned(DealId Id, StageId FromStageId, StageId ToStageId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
