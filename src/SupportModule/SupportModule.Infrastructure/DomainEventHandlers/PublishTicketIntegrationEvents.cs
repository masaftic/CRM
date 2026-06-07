using MediatR;
using Shared.Infrastructure.IntegrationEvents;
using SupportModule.Contracts.Tickets.Events;
using SupportModule.Domain.Events;

namespace SupportModule.Infrastructure.DomainEventHandlers;

public sealed class PublishTicketCreatedIntegrationEvent(IIntegrationEventPublisher<SupportModuleMarker> integrationEventPublisher)
    : INotificationHandler<TicketCreated>
{
    public Task Handle(TicketCreated notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new TicketCreatedIntegrationEvent(
            notification.TicketId.ToString(),
            notification.CustomerId,
            notification.Title,
            notification.Priority.ToString(),
            notification.OccurredOn));

        return Task.CompletedTask;
    }
}

public sealed class PublishTicketAssignedIntegrationEvent(IIntegrationEventPublisher<SupportModuleMarker> integrationEventPublisher)
    : INotificationHandler<TicketAssigned>
{
    public Task Handle(TicketAssigned notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new TicketAssignedIntegrationEvent(
            notification.TicketId.ToString(),
            notification.PreviousAgentId,
            notification.AgentId,
            notification.AssignedBy,
            notification.OccurredOn));

        return Task.CompletedTask;
    }
}

public sealed class PublishTicketResolvedIntegrationEvent(IIntegrationEventPublisher<SupportModuleMarker> integrationEventPublisher)
    : INotificationHandler<TicketResolved>
{
    public Task Handle(TicketResolved notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new TicketResolvedIntegrationEvent(
            notification.TicketId.ToString(),
            notification.ResolvedBy,
            notification.OccurredOn));

        return Task.CompletedTask;
    }
}
