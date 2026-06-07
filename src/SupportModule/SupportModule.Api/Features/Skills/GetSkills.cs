using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SupportModule.Infrastructure.Data;
using SupportModule.Infrastructure.Data.Queries;

namespace SupportModule.Api.Features.Skills;

public static class GetSkills
{
    public static async Task<IResult> Handle(SupportDbContext db, CancellationToken ct)
    {
        var skills = await db.Skills
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToSkillResponses()
            .ToListAsync(ct);

        return Results.Ok(skills);
    }
}
