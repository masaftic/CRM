using SupportModule.Contracts.Tickets.Responses;
using SupportModule.Domain.TicketCommentRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Infrastructure.Data.Queries;

public static class TicketProjections
{
    public static IQueryable<TicketResponse> ToTicketResponses(this IQueryable<Ticket> query)
        => query.Select(ticket => new TicketResponse(
            ticket.TicketId.ToString(),
            ticket.CustomerId,
            ticket.CategoryId.ToString(),
            ticket.AssignedAgentId,
            ticket.Title,
            ticket.Description,
            ticket.Status.ToString(),
            ticket.Priority.ToString(),
            ticket.CreatedAtUtc,
            ticket.ResolvedAtUtc,
            ticket.ClosedAtUtc));

    public static IQueryable<TicketCommentResponse> ToTicketCommentResponses(this IQueryable<TicketComment> query)
        => query.Select(comment => new TicketCommentResponse(
            comment.Id.ToString(),
            comment.TicketId.ToString(),
            comment.Kind.ToString(),
            comment.CustomerId,
            comment.AgentId,
            comment.Content,
            comment.CreatedAtUtc));
}
