using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using SalesModule.Infrastructure.Data.Queries;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class ReopenDeal
{
    public static async Task<IResult> Handle(string dealId, ReopenDealRequest request, [FromServices] ISalesUnitOfWork uow)
    {
        var dealRepo = uow.GetRepository<Deal, DealId>();
        var deal = await dealRepo.TryFindAsync(DealId.Create(dealId));
        if (deal is null) return AppError.NotFound("Deal.NotFound", "Deal not found").ToProblemDetails();

        try
        {
            deal.Reopen(StageId.Create(request.StageId));
            await uow.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return AppError.Validation("Deal.Reopen.Invalid", ex.Message).ToProblemDetails();
        }

        return Results.Ok(deal.ToResponse());
    }
}
