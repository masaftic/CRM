using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Contracts.Deals.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using SalesModule.Infrastructure.Data.Queries;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Deals;

public static class CreateDeal
{
    public static async Task<IResult> Handle(CreateDealRequest request, [FromServices] ISalesUnitOfWork uow)
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

        return Results.Created($"/sales/deals/{deal.Id}", deal.ToResponse());
    }
}
