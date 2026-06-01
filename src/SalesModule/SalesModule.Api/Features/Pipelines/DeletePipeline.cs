using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class DeletePipeline
{
    public static async Task<Results<Ok, NotFound>> Handle(string pipelineId, IUnitOfWork uow)
    {
        // Validate the pipelineId format
        var validationError = PipelineId.Validate(pipelineId, null, out var validPipelineId);
        if (validationError != null)
        {
            return TypedResults.NotFound();
        }

        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var exists = await repository.TryFindAsync(validPipelineId) is not null;

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(validPipelineId);
        await uow.SaveChangesAsync();

        return TypedResults.Ok();
    }
}