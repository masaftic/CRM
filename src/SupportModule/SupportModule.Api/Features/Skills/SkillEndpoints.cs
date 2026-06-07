using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;
using SupportModule.Contracts.Skills.Requests;

namespace SupportModule.Api.Features.Skills;

public static class SkillEndpoints
{
    public static RouteGroupBuilder MapSkillEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateSkill.Handle)
            .WithValidation<CreateSkillRequest>();
        routes.MapGet("/", GetSkills.Handle);

        return routes;
    }
}
