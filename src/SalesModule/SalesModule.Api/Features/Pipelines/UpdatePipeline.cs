using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class UpdatePipeline
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, UpdatePipelineRequest request, IUnitOfWork uow)
    {
        // Validate the pipelineId format
        if (!PipelineId.Validate(pipelineId, null, out var validPipelineId))
        {
            return TypedResults.BadRequest("Invalid pipeline ID format.");
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
            return TypedResults.BadRequest("Pipeline name is required.");
        }

        pipeline.UpdateName(request.Name);

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