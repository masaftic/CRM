using Microsoft.AspNetCore.Http;
using SupportModule.Contracts.Skills.Requests;
using SupportModule.Domain.SkillRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.Skills;

public static class CreateSkill
{
    public static async Task<IResult> Handle(CreateSkillRequest request, SupportDbContext db, CancellationToken ct)
    {
        var skill = new Skill(request.Name, isActive: true);

        db.Skills.Add(skill);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/support/skills/{skill.Id}", skill.ToResponse());
    }
}
