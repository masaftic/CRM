using System.Text.Json;

namespace Shared.Infrastructure.Data.Outbox;


/// <summary>
/// Used for integration events, later with a real message broker
/// </summary>
public class OutboxMessage
{
    public int Id { get; set; }
    public required Type Type { get; set; }
    public required string JsonData { get; set; }
    public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedOn { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }

    public static OutboxMessage From(object message, JsonSerializerOptions? options = null)
    {
        var messageType = message.GetType();

        return new OutboxMessage
        {
            Type = messageType,
            JsonData = JsonSerializer.Serialize(message, messageType, options),
            OccurredOn = DateTimeOffset.UtcNow
        };
    }
}
