using MediatR;
using SalesModule.Contracts.Deals.Events;
using SalesModule.Domain.Events;
using Shared.Infrastructure.IntegrationEvents;

namespace SalesModule.Infrastructure.DomainEventHandlers;

public sealed class PublishDealCreatedIntegrationEvent(IIntegrationEventPublisher integrationEventPublisher)
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
