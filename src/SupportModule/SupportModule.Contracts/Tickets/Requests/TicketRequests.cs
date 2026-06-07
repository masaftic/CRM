namespace SupportModule.Contracts.Tickets.Requests;

public record CreateTicketRequest(Guid CategoryId, Guid CustomerId, string Title, string Description);

public record AssignTicketRequest(Guid AgentId, Guid AssignedBy, string? Reason);

public record ResolveTicketRequest(Guid ResolvedBy, string? Summary);

public record AddTicketCommentRequest(Guid ActorId, string Content);
