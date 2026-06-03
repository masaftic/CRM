namespace SupportModule.Domain.TicketRoot;

public class TicketTransition
{
    public int Id { get; init; }
    public TicketId TicketId { get; init; } = default;
    public TicketStatus From { get; init; }
    public TicketStatus To { get; init; }
    public DateTime TransitionedAtUtc { get; init; }
    public Guid TransitionedBy { get; init; }

    private TicketTransition() { }

    public TicketTransition(TicketId ticketId, TicketStatus from, TicketStatus to, DateTime transitionedAtUtc, Guid transitionedBy)
    {
        TicketId = ticketId;
        From = from;
        To = to;
        TransitionedAtUtc = transitionedAtUtc;
        TransitionedBy = transitionedBy;
    }
}
