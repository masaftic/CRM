using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.Tickets.Requests;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.Tickets;

public static class ResolveTicket
{
    public static async Task<IResult> Handle(Guid ticketId, ResolveTicketRequest request, SupportDbContext db, CancellationToken ct)
    {
        var id = TicketId.Create(ticketId);
        var ticket = await db.Tickets
            .Include(x => x.Transitions)
            .FirstOrDefaultAsync(x => x.TicketId == id, ct);

        if (ticket is null)
            return AppError.NotFound("Ticket.NotFound", "Ticket not found.").ToProblemDetails();

        var result = ticket.ResolveTicket(request.ResolvedBy, request.Summary);
        if (result.IsError) return result.Errors.ToProblemDetails();

        if (ticket.AssignedAgentId is not null)
        {
            var profile = await db.SupportAgentProfiles.FirstOrDefaultAsync(x => x.AgentId == ticket.AssignedAgentId, ct);
            profile?.ReleaseTicket(ticket.TicketId, request.ResolvedBy);
        }

        await db.SaveChangesAsync(ct);

        return Results.Ok(ticket.ToResponse());
    }
}
