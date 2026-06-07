using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;
using SupportModule.Contracts.TicketCategories.Requests;

namespace SupportModule.Api.Features.TicketCategories;

public static class TicketCategoryEndpoints
{
    public static RouteGroupBuilder MapTicketCategoryEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateTicketCategory.Handle)
            .WithValidation<CreateTicketCategoryRequest>();
        routes.MapGet("/", GetTicketCategories.Handle);
        routes.MapPost("{categoryId:guid}/required-skills", AddRequiredSkill.Handle)
            .WithValidation<AddRequiredSkillRequest>();

        return routes;
    }
}
