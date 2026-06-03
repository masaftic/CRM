namespace SupportModule.Domain.TicketRoot;

public class TicketAssignment
{
    public int Id { get; init; }
    public TicketId TicketId { get; init; } = default;
    public Guid? PreviousAgentId { get; init; }
    public Guid AgentId { get; init; }
    public Guid AssignedBy { get; init; }
    public DateTime AssignedAtUtc { get; init; }
    public string? Reason { get; private set; }

    private TicketAssignment() { }

    public TicketAssignment(TicketId ticketId, Guid? previousAgentId, Guid agentId, Guid assignedBy, string? reason = null)
        => (TicketId, PreviousAgentId, AgentId, AssignedAtUtc, AssignedBy, Reason)
         = (ticketId, previousAgentId, agentId, DateTime.UtcNow, assignedBy, reason);
}
