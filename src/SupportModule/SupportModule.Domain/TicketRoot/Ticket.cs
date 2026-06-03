using BuildingBlocks.Domain;
using SupportModule.Domain.Events;
using Thinktecture;

namespace SupportModule.Domain.TicketRoot;



[ValueObject<Guid>]
public partial struct TicketId
{
    public static TicketId New() => Create(Guid.NewGuid());
}


public class Ticket : AggregateRoot
{
    public TicketId TicketId { get; init; } = TicketId.New();

    public Guid CustomerId { get; init; }

    public Guid? AssignedAgentId { get; private set; }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; private set; }
    public Guid? ResolvedBy { get; private set; }
    public string? ResolutionSummary { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public Guid? ClosedBy { get; private set; }

    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; } = TicketPriority.Low;

    private readonly List<TicketNote> _notes = [];
    public IReadOnlyCollection<TicketNote> Notes => _notes.AsReadOnly();

    private readonly List<TicketAssignment> _assignments = [];
    public IReadOnlyCollection<TicketAssignment> Assignments => _assignments.AsReadOnly();

    private readonly List<TicketTransition> _transitions = [];
    public IReadOnlyCollection<TicketTransition> Transitions => _transitions.AsReadOnly();

    private readonly List<TicketPriorityChange> _priorityChanges = [];
    public IReadOnlyCollection<TicketPriorityChange> PriorityChanges => _priorityChanges.AsReadOnly();


    private Ticket() { }


    public Ticket(Guid customerId, string title, string description)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId must be provided.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must be provided.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must be provided.", nameof(description));

        CustomerId = customerId;
        Title = title.Trim();
        Description = description.Trim();

        Status = TicketStatus.Open;

        RaiseDomainEvent(new TicketCreated(TicketId, CustomerId, Title, Priority));
    }

    public void AssignAgent(Guid agentId, Guid assignedBy, string? reason = null)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("AgentId must be provided.", nameof(agentId));
        if (assignedBy == Guid.Empty)
            throw new ArgumentException("AssignedBy must be provided.", nameof(assignedBy));

        if (AssignedAgentId.HasValue && AssignedAgentId == agentId) return;

        var previousAgentId = AssignedAgentId;
        _assignments.Add(new TicketAssignment(TicketId, previousAgentId, agentId, assignedBy, reason));
        AssignedAgentId = agentId;

        RaiseDomainEvent(new TicketAssigned(TicketId, previousAgentId, agentId, assignedBy));
    }

    public void ChangePriority(TicketPriority priority, Guid changedBy, string? reason = null)
    {
        if (changedBy == Guid.Empty)
            throw new ArgumentException("ChangedBy must be provided.", nameof(changedBy));
        if (priority == TicketPriority.Urgent && string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Urgent priority changes require a reason.");
        if (priority == Priority) return;

        var previousPriority = Priority;
        _priorityChanges.Add(new TicketPriorityChange(TicketId, previousPriority, priority, changedBy, reason));
        Priority = priority;

        RaiseDomainEvent(new TicketPriorityChanged(TicketId, previousPriority, priority, changedBy));
    }

    public void AddCustomerReply(string message)
    {
        EnsureMessageProvided(message);

        _notes.Add(new CustomerTicketReply
        {
            TicketId = TicketId,
            Content = message.Trim(),
        });

        RaiseDomainEvent(new CustomerRepliedToTicket(TicketId, CustomerId));
    }

    public void AddAgentReply(string message, Guid agentId)
    {
        EnsureMessageProvided(message);
        EnsureActorProvided(agentId, nameof(agentId));

        _notes.Add(new AgentTicketReply
        {
            TicketId = TicketId,
            Content = message.Trim(),
            AgentId = agentId
        });

        RaiseDomainEvent(new AgentRepliedToTicket(TicketId, agentId));
    }

    public void AddInternalNote(string message, Guid addedBy)
    {
        EnsureMessageProvided(message);
        EnsureActorProvided(addedBy, nameof(addedBy));

        _notes.Add(new InternalTicketNote
        {
            TicketId = TicketId,
            Content = message.Trim(),
            AddedBy = addedBy
        });

        RaiseDomainEvent(new InternalTicketNoteAdded(TicketId, addedBy));
    }

    public void CloseTicket(Guid closedBy)
    {
        EnsureActorProvided(closedBy, nameof(closedBy));

        TransitionTo(TicketStatus.Closed, closedBy);
        ClosedAtUtc = DateTime.UtcNow;
        ClosedBy = closedBy;

        RaiseDomainEvent(new TicketClosed(TicketId, closedBy));
    }

    public void ReOpenTicket(Guid reOpenedBy)
    {
        EnsureActorProvided(reOpenedBy, nameof(reOpenedBy));

        TransitionTo(TicketStatus.Open, reOpenedBy);
        ResolvedAtUtc = null;
        ResolvedBy = null;
        ResolutionSummary = null;
        ClosedAtUtc = null;
        ClosedBy = null;

        RaiseDomainEvent(new TicketReopened(TicketId, reOpenedBy));
    }

    public void ResolveTicket(Guid resolvedBy, string? resolutionSummary = null)
    {
        EnsureActorProvided(resolvedBy, nameof(resolvedBy));

        TransitionTo(TicketStatus.Resolved, resolvedBy);
        ResolvedAtUtc = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionSummary = string.IsNullOrWhiteSpace(resolutionSummary) ? null : resolutionSummary.Trim();

        RaiseDomainEvent(new TicketResolved(TicketId, resolvedBy));
    }

    public void StartProgress(Guid agentId)
    {
        EnsureActorProvided(agentId, nameof(agentId));

        TransitionTo(TicketStatus.InProgress, agentId);
    }

    public void WaitForCustomer(Guid agentId)
    {
        EnsureActorProvided(agentId, nameof(agentId));

        TransitionTo(TicketStatus.WaitingForCustomer, agentId);
    }

    private void TransitionTo(TicketStatus to, Guid transitionedBy)
    {
        if (Status == to) return;
        if (!CanTransition(Status, to))
            throw new InvalidOperationException($"Cannot transition ticket from {Status} to {to}.");

        var from = Status;
        _transitions.Add(new TicketTransition
        (
            ticketId: TicketId,
            from: from,
            to: to,
            transitionedAtUtc: DateTime.UtcNow,
            transitionedBy: transitionedBy
        ));

        Status = to;

        RaiseDomainEvent(new TicketStatusChanged(TicketId, from, to, transitionedBy));
    }

    private static bool CanTransition(TicketStatus from, TicketStatus to) =>
        (from, to) switch
        {
            (TicketStatus.Open, TicketStatus.InProgress) => true,
            (TicketStatus.Open, TicketStatus.WaitingForCustomer) => true,
            (TicketStatus.Open, TicketStatus.Resolved) => true,
            (TicketStatus.InProgress, TicketStatus.WaitingForCustomer) => true,
            (TicketStatus.InProgress, TicketStatus.Resolved) => true,
            (TicketStatus.WaitingForCustomer, TicketStatus.InProgress) => true,
            (TicketStatus.WaitingForCustomer, TicketStatus.Resolved) => true,
            (TicketStatus.Resolved, TicketStatus.Closed) => true,
            (TicketStatus.Resolved, TicketStatus.Open) => true,
            (TicketStatus.Closed, TicketStatus.Open) => true,
            _ => false
        };

    private static void EnsureMessageProvided(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message must be provided.", nameof(message));
    }

    private static void EnsureActorProvided(Guid actorId, string paramName)
    {
        if (actorId == Guid.Empty)
            throw new ArgumentException("Actor id must be provided.", paramName);
    }
}
