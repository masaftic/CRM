namespace Shared.Infrastructure.IntegrationEvents;

public interface IIntegrationEventPublisher<TModule>
{
    void Publish(object integrationEvent);
}
