using MediatR;

namespace BuildingBlocks.Domain;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}

public record DomainEvent : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
