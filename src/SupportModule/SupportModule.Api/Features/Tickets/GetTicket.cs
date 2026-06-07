using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.Tickets;

public static class GetTicket
{
    public static async Task<IResult> Handle(Guid ticketId, SupportDbContext db, CancellationToken ct)
    {
        var id = TicketId.Create(ticketId);
        var ticket = await db.Tickets
            .AsNoTracking()
            .Where(x => x.TicketId == id)
            .ToTicketResponses()
            .FirstOrDefaultAsync(ct);

        return ticket is null
            ? AppError.NotFound("Ticket.NotFound", "Ticket not found.").ToProblemDetails()
            : Results.Ok(ticket);
    }
}
