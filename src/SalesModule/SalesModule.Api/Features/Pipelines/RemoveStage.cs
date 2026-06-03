using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class RemoveStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, string stageId, [FromServices] ISalesUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            pipeline.RemoveStage(StageId.Create(stageId));
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

        await uow.SaveChangesAsync();

        return TypedResults.Ok(pipeline.ToResponse());
    }
}