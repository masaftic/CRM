using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.Tickets;

public static class GetTickets
{
    public static async Task<IResult> Handle(SupportDbContext db, CancellationToken ct)
    {
        var tickets = await db.Tickets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToTicketResponses()
            .ToListAsync(ct);

        return Results.Ok(tickets);
    }
}
