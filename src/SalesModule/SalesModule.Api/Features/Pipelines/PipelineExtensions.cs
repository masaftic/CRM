using SalesModule.Contracts.Pipelines.Responses;
using SalesModule.Domain;

namespace SalesModule.Api.Features.Pipelines;

public static class PipelineExtensions
{
    extension(Pipeline pipeline)
    {
        public PipelineResponse ToResponse() => new()
        {
            PipelineId = pipeline.Id,
            Name = pipeline.Name,
            Stages = pipeline.Stages.Select(s => new StageResponse
            {
                StageId = s.Id,
                Name = s.Name,
                Order = s.Order,
                Probability = s.Probability
            }).ToList()
        };
    } 
}
