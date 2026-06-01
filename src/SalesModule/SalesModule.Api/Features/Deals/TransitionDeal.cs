using Microsoft.AspNetCore.Http;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;

namespace SalesModule.Api.Features.Deals;

public static class TransitionDeal
{
    public static IResult Handle(DealId dealId, TransitionDealRequest request)
    {
        return Results.Ok();
    } 
}
