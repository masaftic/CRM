using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class AddStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, AddStageRequest request, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        var currentStageCount = pipeline.Stages.Count;
        var stage = new Stage(StageId.NewId(), pipeline.Id, request.Name, 0, request.Probability);
        pipeline.AddStage(stage); // Adds to the end

        // If the desired order is not the end, move the stage to the desired position
        if (request.Order < currentStageCount)
        {
            pipeline.MoveStage(stage.Id, request.Order);
        }

        await uow.SaveChangesAsync();

        return TypedResults.Ok(pipeline.ToResponse());
    }
}