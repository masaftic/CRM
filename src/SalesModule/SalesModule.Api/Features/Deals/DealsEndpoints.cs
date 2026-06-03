using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SalesModule.Contracts.Deals.Requests;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class DealsEndpoints
{
    public static void MapDealEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateDeal.Handle)
            .WithValidation<CreateDealRequest>();
        routes.MapGet("/", GetDeals.Handle);
        routes.MapGet("{dealId}", GetDeal.Handle);
        routes.MapPatch("{dealId}/stage", TransitionDeal.Handle)
            .WithValidation<TransitionDealRequest>();
        routes.MapPost("{dealId}/move-forward", MoveDealForward.Handle);
        routes.MapPost("{dealId}/move-backward", MoveDealBackward.Handle);
        routes.MapPost("{dealId}/win", MarkDealWon.Handle);
        routes.MapPost("{dealId}/lose", MarkDealLost.Handle);
        routes.MapPost("{dealId}/reopen", ReopenDeal.Handle)
            .WithValidation<ReopenDealRequest>();
        routes.MapDelete("{dealId}", DeleteDeal.Handle);
    }   
}
