namespace Shared.Infrastructure.IntegrationEvents;

public interface IIntegrationEventHandler<in TIntegrationEvent>
{
    Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
