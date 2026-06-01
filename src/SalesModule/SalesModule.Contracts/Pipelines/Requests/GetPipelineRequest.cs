namespace SalesModule.Contracts.Pipelines.Requests;

public record GetPipelineRequest
{
    public string PipelineId { get; init; } = string.Empty;
}