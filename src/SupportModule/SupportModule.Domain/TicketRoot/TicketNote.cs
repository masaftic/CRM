namespace SupportModule.Domain.TicketRoot;

public abstract class TicketNote
{
    public int Id { get; init; }
    public TicketId TicketId { get; init; } = default!;
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
}

public class CustomerTicketReply : TicketNote
{
    public string Content { get; init; } = null!;
}

public class AgentTicketReply : TicketNote
{
    public Guid AgentId { get; init; }
    public string Content { get; init; } = null!;
}

public class InternalTicketNote : TicketNote
{
    public Guid AddedBy { get; init; }
    public string Content { get; init; } = null!;
}

