namespace SupportModule.Contracts.Tickets.Responses;

public record TicketResponse(
    string TicketId,
    Guid CustomerId,
    string CategoryId,
    Guid? AssignedAgentId,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime CreatedAtUtc,
    DateTime? ResolvedAtUtc,
    DateTime? ClosedAtUtc);

public record TicketCommentResponse(
    string CommentId,
    string TicketId,
    string Kind,
    Guid? CustomerId,
    Guid? AgentId,
    string Content,
    DateTime CreatedAtUtc);
