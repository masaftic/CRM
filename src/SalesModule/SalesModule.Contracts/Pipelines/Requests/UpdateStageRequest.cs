namespace SalesModule.Contracts.Pipelines.Requests;

public record UpdateStageRequest
{
    public string Name { get; init; } = string.Empty;
    public decimal Probability { get; init; }
}