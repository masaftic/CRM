using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.Tickets.Requests;
using SupportModule.Domain.TicketCommentRoot;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.Tickets;

public static class AddCustomerComment
{
    public static async Task<IResult> Handle(Guid ticketId, AddTicketCommentRequest request, SupportDbContext db, CancellationToken ct)
    {
        var id = TicketId.Create(ticketId);
        var ticket = await db.Tickets
            .AsNoTracking()
            .Where(x => x.TicketId == id)
            .Select(x => new { x.TicketId, x.CustomerId, x.Status })
            .FirstOrDefaultAsync(ct);

        if (ticket is null)
            return AppError.NotFound("Ticket.NotFound", "Ticket not found.").ToProblemDetails();
        if (ticket.Status == TicketStatus.Closed)
            return AppError.Conflict("Ticket.Closed", "Cannot add a comment to a closed ticket.").ToProblemDetails();
        if (ticket.CustomerId != request.ActorId)
            return AppError.Forbidden("Ticket.CustomerMismatch", "Only the ticket customer can add a customer reply.").ToProblemDetails();

        var result = TicketComment.CustomerReply(id, request.ActorId, request.Content);
        if (result.IsError) return result.Errors.ToProblemDetails();

        db.TicketComments.Add(result.Value);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/support/tickets/{ticketId}/comments/{result.Value.Id}", result.Value.ToResponse());
    }
}
