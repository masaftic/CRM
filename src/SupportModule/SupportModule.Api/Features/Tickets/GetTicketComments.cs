using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.Tickets;

public static class GetTicketComments
{
    public static async Task<IResult> Handle(Guid ticketId, SupportDbContext db, CancellationToken ct)
    {
        var id = TicketId.Create(ticketId);
        var comments = await db.TicketComments
            .AsNoTracking()
            .Where(x => x.TicketId == id)
            .OrderBy(x => x.CreatedAtUtc)
            .ToTicketCommentResponses()
            .ToListAsync(ct);

        return Results.Ok(comments);
    }
}
