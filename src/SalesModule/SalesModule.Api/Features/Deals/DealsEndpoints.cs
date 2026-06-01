using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SalesModule.Api.Features.Deals;

public static class DealsEndpoints
{
    public static void MapDealEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/", CreateDeal.Handle);
        routes.MapPatch("{dealId}/move", TransitionDeal.Handle);
        routes.MapPost("{dealId}/win", MarkDealWon.Handle);
        routes.MapPost("{dealId}/lose", MarkDealLost.Handle);
    }   
}
