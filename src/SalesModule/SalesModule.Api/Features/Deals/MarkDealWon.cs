using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using SalesModule.Domain;
using Shared.Web.Extensions;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Deals;

public static class MarkDealWon
{
    public static async Task<IResult> Handle(DealId dealId, IUnitOfWork uow)
    {
        var dealRepo = uow.GetRepository<Deal, DealId>();
        var deal = await dealRepo.TryFindAsync(dealId);
        if (deal is null) return AppError.NotFound("Deal.NotFound", "Deal not found").ToProblemDetails();

        deal.MarkAsWon();
        await uow.SaveChangesAsync();

        return Results.Ok();
    } 
}
