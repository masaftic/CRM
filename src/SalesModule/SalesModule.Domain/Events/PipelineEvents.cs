using BuildingBlocks.Domain;

namespace SalesModule.Domain.Events;

public record PipelineCreated(PipelineId PipelineId, string Name) : DomainEvent;

public record StageAdded(PipelineId PipelineId, StageId StageId) : DomainEvent;

public record StageRemoved(PipelineId PipelineId, StageId StageId) : DomainEvent;

public record StageReordered(PipelineId PipelineId) : DomainEvent;

public record StageUpdated(PipelineId PipelineId, StageId StageId) : DomainEvent;
