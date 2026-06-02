namespace Shared.Infrastructure.Messaging;

public interface IMessageBus
{
    Task PublishAsync(object message, Type messageType, CancellationToken cancellationToken = default);

    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        return PublishAsync(message, typeof(TMessage), cancellationToken);
    }
}
