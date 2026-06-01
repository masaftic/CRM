namespace SalesModule.Contracts.Pipelines.Responses;

public record PipelineResponse
{
    public string PipelineId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public List<StageResponse> Stages { get; init; } = [];

}

public record StageResponse
{
    public string StageId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public decimal Probability { get; init; }
}