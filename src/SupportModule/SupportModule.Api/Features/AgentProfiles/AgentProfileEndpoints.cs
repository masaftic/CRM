using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;
using SupportModule.Contracts.AgentProfiles.Requests;

namespace SupportModule.Api.Features.AgentProfiles;

public static class AgentProfileEndpoints
{
    public static RouteGroupBuilder MapAgentProfileEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateAgentProfile.Handle)
            .WithValidation<CreateAgentProfileRequest>();
        routes.MapGet("/", GetAgentProfiles.Handle);
        routes.MapPatch("{profileId:guid}/availability", ChangeAgentAvailability.Handle)
            .WithValidation<ChangeAgentAvailabilityRequest>();
        routes.MapPatch("{profileId:guid}/capacity", ChangeAgentCapacity.Handle)
            .WithValidation<ChangeAgentCapacityRequest>();

        return routes;
    }
}
