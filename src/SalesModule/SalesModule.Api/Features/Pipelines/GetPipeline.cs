using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class GetPipeline
{
    public static async Task<Results<Ok<PipelineResponse>, NotFound>> Handle(string pipelineId, GetPipelineRequest request, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(pipeline.ToResponse());
    }
}