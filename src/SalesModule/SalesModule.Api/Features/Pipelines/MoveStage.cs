using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class MoveStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, string stageId, int newIndex, [FromServices] ISalesUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        var stageCount = pipeline.Stages.Count;
        if (newIndex < 0 || newIndex >= stageCount)
        {
            return TypedResults.BadRequest($"New index must be between 0 and {stageCount - 1}.");
        }

        try
        {
            pipeline.MoveStage(StageId.Create(stageId), newIndex);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

        await uow.SaveChangesAsync();

        return TypedResults.Ok(pipeline.ToResponse());
    }
}