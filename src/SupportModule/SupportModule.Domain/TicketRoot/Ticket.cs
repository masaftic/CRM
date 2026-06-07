using BuildingBlocks.Domain;
using SupportModule.Domain.Events;
using SupportModule.Domain.TicketCategoryRoot;
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

    public TicketCategoryId CategoryId { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; } = TicketPriority.Low;

    private readonly List<TicketAssignment> _assignments = [];
    public IReadOnlyCollection<TicketAssignment> Assignments => _assignments.AsReadOnly();

    private readonly List<TicketTransition> _transitions = [];
    public IReadOnlyCollection<TicketTransition> Transitions => _transitions.AsReadOnly();

    private readonly List<TicketPriorityChange> _priorityChanges = [];
    public IReadOnlyCollection<TicketPriorityChange> PriorityChanges => _priorityChanges.AsReadOnly();


    private Ticket() { }


    public Ticket(TicketCategoryId categoryId, Guid customerId, string title, string description, TicketPriority priority = TicketPriority.Low)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId must be provided.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must be provided.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must be provided.", nameof(description));

        CategoryId = categoryId;
        CustomerId = customerId;
        Title = title.Trim();
        Description = description.Trim();
        Priority = priority;

        Status = TicketStatus.Open;

        RaiseDomainEvent(new TicketCreated(TicketId, CustomerId, Title, Priority));
    }

    public Result AssignAgent(Guid agentId, Guid assignedBy, string? reason = null)
    {
        if (agentId == Guid.Empty)
            return TicketErrors.ActorRequired;
        if (assignedBy == Guid.Empty)
            return TicketErrors.ActorRequired;

        if (AssignedAgentId.HasValue && AssignedAgentId == agentId) return Result.Ok();

        var previousAgentId = AssignedAgentId;
        _assignments.Add(new TicketAssignment(TicketId, previousAgentId, agentId, assignedBy, reason));
        AssignedAgentId = agentId;

        RaiseDomainEvent(new TicketAssigned(TicketId, previousAgentId, agentId, assignedBy));

        return Result.Ok();
    }

    public Result ChangePriority(TicketPriority priority, Guid changedBy, string? reason = null)
    {
        if (changedBy == Guid.Empty)
            return TicketErrors.ActorRequired;
        if (priority == TicketPriority.Urgent && string.IsNullOrWhiteSpace(reason))
            return TicketErrors.UrgentPriorityRequiresReason;
        if (priority == Priority) return Result.Ok();

        var previousPriority = Priority;
        _priorityChanges.Add(new TicketPriorityChange(TicketId, previousPriority, priority, changedBy, reason));
        Priority = priority;

        RaiseDomainEvent(new TicketPriorityChanged(TicketId, previousPriority, priority, changedBy));

        return Result.Ok();
    }

    public Result CloseTicket(Guid closedBy)
    {
        var transitionResult = TransitionTo(TicketStatus.Closed, closedBy);
        if (transitionResult.IsError) return transitionResult;

        ClosedAtUtc = DateTime.UtcNow;
        ClosedBy = closedBy;

        RaiseDomainEvent(new TicketClosed(TicketId, closedBy));

        return Result.Ok();
    }

    public Result ReOpenTicket(Guid reOpenedBy)
    {
        var transitionResult = TransitionTo(TicketStatus.Open, reOpenedBy);
        if (transitionResult.IsError) return transitionResult;

        ResolvedAtUtc = null;
        ResolvedBy = null;
        ResolutionSummary = null;
        ClosedAtUtc = null;
        ClosedBy = null;

        RaiseDomainEvent(new TicketReopened(TicketId, reOpenedBy));

        return Result.Ok();
    }

    public Result ResolveTicket(Guid resolvedBy, string? resolutionSummary = null)
    {
        var transitionResult = TransitionTo(TicketStatus.Resolved, resolvedBy);
        if (transitionResult.IsError) return transitionResult;

        ResolvedAtUtc = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionSummary = string.IsNullOrWhiteSpace(resolutionSummary) ? null : resolutionSummary.Trim();

        RaiseDomainEvent(new TicketResolved(TicketId, resolvedBy));

        return Result.Ok();
    }

    public Result StartProgress(Guid agentId)
    {
        return TransitionTo(TicketStatus.InProgress, agentId);
    }

    public Result WaitForCustomer(Guid agentId)
    {
        return TransitionTo(TicketStatus.WaitingForCustomer, agentId);
    }

    private Result TransitionTo(TicketStatus to, Guid transitionedBy)
    {
        if (transitionedBy == Guid.Empty)
            return TicketErrors.ActorRequired;
        if (Status == to) return Result.Ok();
        if (!CanTransition(Status, to))
            return TicketErrors.InvalidTransition(Status, to);

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

        return Result.Ok();
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

}
