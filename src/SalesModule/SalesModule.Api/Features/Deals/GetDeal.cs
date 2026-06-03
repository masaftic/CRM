using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using SalesModule.Infrastructure.Data.Queries;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class GetDeal
{
    public static async Task<IResult> Handle(string dealId, IDealsQueries queries)
    {
        var deal = await queries.GetDeal(dealId);
        if (deal is null) return AppError.NotFound("Deal.NotFound", "Deal not found").ToProblemDetails();

        return Results.Ok(deal);
    }
}
