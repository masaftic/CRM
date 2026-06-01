using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using FluentValidation;

namespace SalesModule.Api.Features.Pipelines;

public static class GetPipeline
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound>> Handle(GetPipelineRequest request, IUnitOfWork uow)
    {
        // Validate the request using FluentValidation
        var validator = new GetPipelineValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            // Return BadRequest with validation errors
            return TypedResults.BadRequest(validationResult.Errors);
        }

        var pipelineId = request.PipelineId;

        // Validate the pipelineId format using our custom validator logic
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