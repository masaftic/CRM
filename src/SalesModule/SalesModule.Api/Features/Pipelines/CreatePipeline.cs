using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class CreatePipeline
{
    public static async Task<IResult> Handle([FromBody] CreatePipelineRequest request, [FromServices] ISalesUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();

        var pipelineId = PipelineId.NewId();
        var stages = request.Stages.Select(s => new Stage(StageId.NewId(), pipelineId, s.Name, s.Order, s.Probability)).ToList();
        var pipeline = new Pipeline(pipelineId, request.Name, stages);

        repository.Add(pipeline);
        await uow.SaveChangesAsync();

        return TypedResults.Ok(pipeline.ToResponse());
    } 
}
