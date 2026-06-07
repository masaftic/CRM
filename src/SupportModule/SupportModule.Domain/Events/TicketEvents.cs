using BuildingBlocks.Domain;
using SupportModule.Domain.TicketCommentRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Domain.Events;

public record TicketCreated(TicketId TicketId, Guid CustomerId, string Title, TicketPriority Priority) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketAssigned(TicketId TicketId, Guid? PreviousAgentId, Guid AgentId, Guid AssignedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketPriorityChanged(TicketId TicketId, TicketPriority From, TicketPriority To, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketStatusChanged(TicketId TicketId, TicketStatus From, TicketStatus To, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record CustomerRepliedToTicket(TicketCommentId CommentId, TicketId TicketId, Guid CustomerId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record AgentRepliedToTicket(TicketCommentId CommentId, TicketId TicketId, Guid AgentId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record InternalTicketNoteAdded(TicketCommentId CommentId, TicketId TicketId, Guid AddedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketResolved(TicketId TicketId, Guid ResolvedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketClosed(TicketId TicketId, Guid ClosedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record TicketReopened(TicketId TicketId, Guid ReopenedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
