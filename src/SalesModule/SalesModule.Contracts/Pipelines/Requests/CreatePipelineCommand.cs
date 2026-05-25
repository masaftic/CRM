namespace SalesModule.Contracts.Pipelines.Requests;

public record CreatePipelineRequest
{
    public string Name { get; init; } = string.Empty;

    public List<CreateStageDto> Stages { get; init; } = [];

    public record CreateStageDto
    {
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; }
        public decimal Probability { get; init; }
    }
}
