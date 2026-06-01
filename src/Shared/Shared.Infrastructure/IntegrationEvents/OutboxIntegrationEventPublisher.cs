using Shared.Infrastructure.Data.Outbox;

namespace Shared.Infrastructure.IntegrationEvents;

public sealed class OutboxIntegrationEventPublisher(IOutboxWriter outboxWriter) : IIntegrationEventPublisher
{
    public void Publish(object integrationEvent)
    {
        outboxWriter.Write(integrationEvent);
    }
}
