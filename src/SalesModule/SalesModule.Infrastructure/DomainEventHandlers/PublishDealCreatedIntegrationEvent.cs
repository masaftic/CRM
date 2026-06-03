using MediatR;
using SalesModule.Contracts.Deals.Events;
using SalesModule.Domain.Events;
using Shared.Infrastructure.IntegrationEvents;

namespace SalesModule.Infrastructure.DomainEventHandlers;

public sealed class PublishDealCreatedIntegrationEvent(IIntegrationEventPublisher<SalesModuleMarker> integrationEventPublisher)
    : INotificationHandler<DealCreated>
{
    public Task Handle(DealCreated notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new DealCreatedIntegrationEvent(
            notification.Id.ToString(),
            notification.Name,
            notification.Amount,
            notification.PipelineId.ToString()));

        return Task.CompletedTask;
    }
}

public sealed class PublishDealTransitionedIntegrationEvent(IIntegrationEventPublisher<SalesModuleMarker> integrationEventPublisher)
    : INotificationHandler<DealTransitioned>
{
    public Task Handle(DealTransitioned notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new DealTransitionedIntegrationEvent(
            notification.Id.ToString(),
            notification.FromStageId.ToString(),
            notification.ToStageId.ToString(),
            notification.OccurredOn));

        return Task.CompletedTask;
    }
}

public sealed class PublishDealWonIntegrationEvent(IIntegrationEventPublisher<SalesModuleMarker> integrationEventPublisher)
    : INotificationHandler<DealWon>
{
    public Task Handle(DealWon notification, CancellationToken cancellationToken)
    {
        integrationEventPublisher.Publish(new DealWonIntegrationEvent(
            notification.Id.ToString(),
            notification.ContactId,
            notification.Name,
            notification.Amount,
            notification.Currency,
            notification.OccurredOn));

        return Task.CompletedTask;
    }
}
