using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class UpdatePipeline
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound, BadRequest<string>>> Handle(string pipelineId, UpdatePipelineRequest request, [FromServices] ISalesUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

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

        return TypedResults.Ok(pipeline.ToResponse());
    }
}