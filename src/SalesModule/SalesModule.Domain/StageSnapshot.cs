using Thinktecture;

namespace SalesModule.Domain;

public class StageSnapshot
{
    public DealId DealId { get; init; } = null!;
    public StageId StageId { get; init; } = null!;
    public PipelineId PipelineId { get; init; } = null!;
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public decimal Probability { get; init; }

    private StageSnapshot() { } // For ORM

    public StageSnapshot(DealId dealId, StageId stageId, PipelineId pipelineId, string name, int order, decimal probability)
    {
        DealId = dealId;
        StageId = stageId;
        PipelineId = pipelineId;
        Name = name;
        Order = order;
        Probability = probability;
    }
}
