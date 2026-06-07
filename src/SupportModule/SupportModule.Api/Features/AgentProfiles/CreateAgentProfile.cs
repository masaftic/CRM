using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.AgentProfiles.Requests;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.AgentProfiles;

public static class CreateAgentProfile
{
    public static async Task<IResult> Handle(CreateAgentProfileRequest request, SupportDbContext db, CancellationToken ct)
    {
        var availability = Enum.Parse<AgentAvailabilityStatus>(request.AvailabilityStatus, true);

        var skillIds = request.SkillIds.Select(SkillId.Create).Distinct().ToList();
        var existingSkillCount = await db.Skills.CountAsync(x => skillIds.Contains(x.Id), ct);
        if (existingSkillCount != skillIds.Count)
            return AppError.Validation("AgentProfile.SkillMissing", "One or more skills do not exist.").ToProblemDetails();

        var alreadyExists = await db.SupportAgentProfiles.AnyAsync(x => x.AgentId == request.AgentId, ct);
        if (alreadyExists)
            return AppError.Conflict("AgentProfile.AgentAlreadyExists", "A support profile already exists for this agent.").ToProblemDetails();

        var profile = new SupportAgentProfile(
            request.AgentId,
            request.Name,
            skillIds,
            availability,
            request.MaxActiveTickets,
            request.IsActive);

        db.SupportAgentProfiles.Add(profile);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/support/agent-profiles/{profile.ProfileId}", profile.ToResponse());
    }
}
