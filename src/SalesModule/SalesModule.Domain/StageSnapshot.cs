namespace SalesModule.Domain;

public record StageSnapshot(StageId Id, PipelineId PipelineId, string Name, int Order, decimal Probability);
