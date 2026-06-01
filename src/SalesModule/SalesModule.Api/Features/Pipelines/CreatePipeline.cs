using Microsoft.AspNetCore.Http;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Domain;
using Shared.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class CreatePipeline
{
    public static async Task<IResult> Handle(CreatePipelineRequest request, IUnitOfWork uow)
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
