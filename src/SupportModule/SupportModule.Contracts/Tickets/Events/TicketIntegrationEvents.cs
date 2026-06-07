namespace SupportModule.Contracts.Tickets.Events;

public record TicketCreatedIntegrationEvent(
    string TicketId,
    Guid CustomerId,
    string Title,
    string Priority,
    DateTimeOffset CreatedAt);

public record TicketAssignedIntegrationEvent(
    string TicketId,
    Guid? PreviousAgentId,
    Guid AgentId,
    Guid AssignedBy,
    DateTimeOffset AssignedAt);

public record TicketResolvedIntegrationEvent(
    string TicketId,
    Guid ResolvedBy,
    DateTimeOffset ResolvedAt);
