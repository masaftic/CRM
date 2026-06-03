using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class UpdateStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, string stageId, UpdateStageRequest request, [FromServices] ISalesUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            pipeline.UpdateStage(StageId.Create(stageId), stage =>
            {
                stage.Name = request.Name;
                stage.Probability = request.Probability;
            });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

        await uow.SaveChangesAsync();

        return TypedResults.Ok(pipeline.ToResponse());
    }
}