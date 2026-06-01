using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class AddStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, AddStageRequest request, IUnitOfWork uow)
    {
        // Validate the pipelineId format
        if (!PipelineId.Validate(pipelineId, null, out var validPipelineId))
        {
            return TypedResults.NotFound();
        }

        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(validPipelineId);

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.BadRequest("Stage name is required.");
        }

        if (request.Probability < 0 || request.Probability > 100)
        {
            return TypedResults.BadRequest("Probability must be between 0 and 100.");
        }

        var currentStageCount = pipeline.Stages.Count;
        if (request.Order < 0 || request.Order > currentStageCount)
        {
            return TypedResults.BadRequest($"Order must be between 0 and {currentStageCount}.");
        }

        var stage = new Stage(StageId.NewId(), pipelineId, request.Name, 0, request.Probability);
        pipeline.AddStage(stage); // Adds to the end

        // If the desired order is not the end, move the stage to the desired position
        if (request.Order < currentStageCount)
        {
            pipeline.MoveStage(stage.Id, request.Order);
        }

        await uow.SaveChangesAsync();

        var response = new PipelineResponse
        {
            PipelineId = pipeline.Id.Value,
            Name = pipeline.Name,
            Stages = pipeline.Stages.Select(s => new PipelineResponse.StageResponse
            {
                StageId = s.Id.Value,
                Name = s.Name,
                Order = s.Order,
                Probability = s.Probability
            }).ToList()
        };

        return TypedResults.Ok(response);
    }
}