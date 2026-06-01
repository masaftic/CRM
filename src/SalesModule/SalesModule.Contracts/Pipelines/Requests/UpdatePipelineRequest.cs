namespace SalesModule.Contracts.Pipelines.Requests;

public record UpdatePipelineRequest
{
    public string Name { get; init; } = string.Empty;
}