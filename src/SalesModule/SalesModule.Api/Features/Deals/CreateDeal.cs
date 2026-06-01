using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;
using Shared.Infrastructure.Data;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class CreateDeal
{
    public static async Task<IResult> Handle(CreateDealRequest request, IUnitOfWork uow)
    {
        var dealRepo = uow.GetRepository<Deal, DealId>();
        var pipelineRepo = uow.GetRepository<Pipeline, PipelineId>();

        var pipeline = await pipelineRepo.TryFindAsync(PipelineId.Create(request.PipelineId));
        if (pipeline is null)
            return AppError.NotFound("Pipeline.NotFound", "Pipeline not found").ToProblemDetails();

        var dealId = DealId.NewId();
        var deal = new Deal(dealId, request.ContactId, request.SalesPersonId, request.Name, Money.Create(request.Value.Amount, request.Value.Currency), request.ExpectedCloseDate, pipeline);

        dealRepo.Add(deal);
        await uow.SaveChangesAsync();

        return Results.Ok();
    }
}
