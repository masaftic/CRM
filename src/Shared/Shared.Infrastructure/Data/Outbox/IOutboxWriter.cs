namespace Shared.Infrastructure.Data.Outbox;

public interface IOutboxWriter
{
    /// <summary>
    /// Writes a message to the outbox, needs a SaveChanges to actually be persisted
    /// </summary>
    /// <param name="message"></param>
    void Write(OutboxMessage message);

    /// <summary>
    /// Wraps a message object as an outbox message, needs a SaveChanges to actually be persisted.
    /// </summary>
    void Write(object message);
}
