namespace SalesModule.Contracts.Pipelines.Requests;

public record AddStageRequest
{
    public string Name { get; init; } = string.Empty;
    public decimal Probability { get; init; }
    public int Order { get; init; }
}