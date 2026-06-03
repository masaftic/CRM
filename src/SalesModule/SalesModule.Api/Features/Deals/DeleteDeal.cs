using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class DeleteDeal
{
    public static async Task<IResult> Handle(string dealId, [FromServices] ISalesUnitOfWork uow)
    {
        var dealRepo = uow.GetRepository<Deal, DealId>();
        var deal = await dealRepo.TryFindAsync(DealId.Create(dealId));
        if (deal is null) return AppError.NotFound("Deal.NotFound", "Deal not found").ToProblemDetails();

        dealRepo.Delete(deal);
        await uow.SaveChangesAsync();

        return Results.NoContent();
    }
}
