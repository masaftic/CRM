using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.AgentProfiles;

public static class GetAgentProfiles
{
    public static async Task<IResult> Handle(SupportDbContext db, CancellationToken ct)
    {
        var profiles = await db.SupportAgentProfiles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToAgentProfileProjection()
            .ToListAsync(ct);

        return Results.Ok(profiles.Select(x => x.ToResponse()));
    }
}
