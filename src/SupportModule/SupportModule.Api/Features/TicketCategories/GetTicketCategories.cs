using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.TicketCategories;

public static class GetTicketCategories
{
    public static async Task<IResult> Handle(SupportDbContext db, CancellationToken ct)
    {
        var categories = await db.TicketCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToTicketCategoryProjection()
            .ToListAsync(ct);

        return Results.Ok(categories.Select(x => x.ToResponse()));
    }
}
