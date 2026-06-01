using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class UpdateStage
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, string stageId, UpdateStageRequest request, IUnitOfWork uow)
    {
        // Validate the pipelineId format
        if (!PipelineId.Validate(pipelineId, null, out var validPipelineId))
        {
            return TypedResults.NotFound();
        }

        // Validate the stageId format
        if (!StageId.Validate(stageId, null, out var validStageId))
        {
            return TypedResults.BadRequest("Invalid stage ID format.");
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

        try
        {
            pipeline.UpdateStage(validStageId, stage =>
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