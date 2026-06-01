using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Domain;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class DeletePipeline
{
    public static async Task<Results<Ok, NotFound>> Handle(string pipelineId, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();
        var pipeline = await repository.TryFindAsync(PipelineId.Create(pipelineId));

        if (pipeline is null)
        {
            return TypedResults.NotFound();
        }

        repository.Delete(pipeline);
        await uow.SaveChangesAsync();

        return TypedResults.Ok();
    }
}