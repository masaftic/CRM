using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.Tickets.Requests;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.Tickets;

public static class AssignTicket
{
    public static async Task<IResult> Handle(Guid ticketId, AssignTicketRequest request, SupportDbContext db, CancellationToken ct)
    {
        var id = TicketId.Create(ticketId);
        var ticket = await db.Tickets
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(x => x.TicketId == id, ct);

        if (ticket is null)
            return AppError.NotFound("Ticket.NotFound", "Ticket not found.").ToProblemDetails();

        var profile = await db.SupportAgentProfiles.FirstOrDefaultAsync(x => x.AgentId == request.AgentId, ct);
        if (profile is null)
            return AppError.NotFound("AgentProfile.NotFound", "Agent profile not found.").ToProblemDetails();

        var previousAgentId = ticket.AssignedAgentId;
        var profileResult = profile.AssignTicket(ticket.TicketId, request.AssignedBy);
        if (profileResult.IsError) return profileResult.Errors.ToProblemDetails();

        var ticketResult = ticket.AssignAgent(request.AgentId, request.AssignedBy, request.Reason);
        if (ticketResult.IsError) return ticketResult.Errors.ToProblemDetails();

        if (previousAgentId is not null && previousAgentId != request.AgentId)
        {
            var previousProfile = await db.SupportAgentProfiles.FirstOrDefaultAsync(x => x.AgentId == previousAgentId, ct);
            previousProfile?.ReleaseTicket(ticket.TicketId, request.AssignedBy);
        }

        await db.SaveChangesAsync(ct);

        return Results.Ok(ticket.ToResponse());
    }
}
