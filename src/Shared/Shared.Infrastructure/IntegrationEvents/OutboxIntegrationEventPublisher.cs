using Shared.Infrastructure.Data.Outbox;

namespace Shared.Infrastructure.IntegrationEvents;

public sealed class OutboxIntegrationEventPublisher<TModule>(IOutboxWriter<TModule> outboxWriter) : IIntegrationEventPublisher<TModule>
{
    public void Publish(object integrationEvent)
    {
        outboxWriter.Write(integrationEvent);
    }
}
