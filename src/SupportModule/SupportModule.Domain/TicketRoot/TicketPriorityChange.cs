namespace SupportModule.Domain.TicketRoot;

public class TicketPriorityChange
{
    public int Id { get; init; }
    public TicketId TicketId { get; init; } = default;
    public TicketPriority From { get; init; }
    public TicketPriority To { get; init; }
    public DateTime TransitionedAtUtc { get; init; } = DateTime.UtcNow;
    public Guid ChangedBy { get; init; }
    public string? Reason { get; private set; }

    private TicketPriorityChange() { }

    public TicketPriorityChange(TicketId ticketId, TicketPriority from, TicketPriority to, Guid changedBy, string? reason = null)
    {
        TicketId = ticketId;
        From = from;
        To = to;
        ChangedBy = changedBy;
        Reason = reason;
    }
}
