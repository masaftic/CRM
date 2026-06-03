using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using SalesModule.Domain;
using Shared.Web.Extensions;
using Shared.Infrastructure.Data;
using SalesModule.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Infrastructure.Data.Queries;

namespace SalesModule.Api.Features.Deals;

public static class MarkDealWon
{
    public static async Task<IResult> Handle(string dealId, [FromServices] ISalesUnitOfWork uow)
    {
        var dealRepo = uow.GetRepository<Deal, DealId>();
        var deal = await dealRepo.TryFindAsync(DealId.Create(dealId));
        if (deal is null) return AppError.NotFound("Deal.NotFound", "Deal not found").ToProblemDetails();

        try
        {
            deal.MarkAsWon();
            await uow.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            return AppError.Validation("Deal.Win.Invalid", ex.Message).ToProblemDetails();
        }

        return Results.Ok(deal.ToResponse());
    } 
}
