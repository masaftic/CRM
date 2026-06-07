using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.AgentProfiles.Requests;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.AgentProfiles;

public static class ChangeAgentCapacity
{
    public static async Task<IResult> Handle(Guid profileId, ChangeAgentCapacityRequest request, SupportDbContext db, CancellationToken ct)
    {
        var profile = await db.SupportAgentProfiles.FirstOrDefaultAsync(x => x.ProfileId == SupportAgentProfileId.Create(profileId), ct);
        if (profile is null)
            return AppError.NotFound("AgentProfile.NotFound", "Agent profile not found.").ToProblemDetails();

        var result = profile.ChangeCapacity(request.MaxActiveTickets, request.ChangedBy);
        if (result.IsError) return result.Errors.ToProblemDetails();

        await db.SaveChangesAsync(ct);

        return Results.Ok(profile.ToResponse());
    }
}
