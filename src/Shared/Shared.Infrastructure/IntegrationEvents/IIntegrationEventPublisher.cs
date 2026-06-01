namespace Shared.Infrastructure.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    void Publish(object integrationEvent);
}
