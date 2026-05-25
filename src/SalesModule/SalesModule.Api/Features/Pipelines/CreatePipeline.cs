using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api.Features.Pipelines;

public static class CreatePipeline
{
    public static async Task<Ok<PipelineResponse>> Handle(CreatePipelineRequest request, IUnitOfWork uow)
    {
        var repository = uow.GetRepository<Pipeline, PipelineId>();

        var pipelineId = PipelineId.NewId();
        var stages = request.Stages.Select(s => new Stage(StageId.NewId(), pipelineId, s.Name, s.Order, s.Probability)).ToList();
        var pipeline = new Pipeline(pipelineId, request.Name, stages);

        repository.Add(pipeline);
        await uow.SaveChangesAsync();

        return TypedResults.Ok(new PipelineResponse
        {
            PipelineId = pipeline.Id,
            Name = pipeline.Name,
            Stages = pipeline.Stages.Select(s => new PipelineResponse.StageResponse
            {
                StageId = s.Id,
                Name = s.Name,
                Order = s.Order,
                Probability = s.Probability
            }).ToList()
        });
    } 
}
