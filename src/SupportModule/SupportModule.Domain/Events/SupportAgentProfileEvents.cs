using BuildingBlocks.Domain;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Domain.Events;

public record SupportAgentProfileCreated(SupportAgentProfileId ProfileId, Guid AgentId, string Name) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentProfileRenamed(SupportAgentProfileId ProfileId, Guid AgentId, string Name, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentProfileActivated(SupportAgentProfileId ProfileId, Guid AgentId, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentProfileDeactivated(SupportAgentProfileId ProfileId, Guid AgentId, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentAvailabilityChanged(
    SupportAgentProfileId ProfileId,
    Guid AgentId,
    AgentAvailabilityStatus From,
    AgentAvailabilityStatus To,
    Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentCapacityChanged(
    SupportAgentProfileId ProfileId,
    Guid AgentId,
    int From,
    int To,
    Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentSkillAdded(SupportAgentProfileId ProfileId, Guid AgentId, SkillId SkillId, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentSkillRemoved(SupportAgentProfileId ProfileId, Guid AgentId, SkillId SkillId, Guid ChangedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentTicketAssigned(SupportAgentProfileId ProfileId, Guid AgentId, TicketId TicketId, Guid AssignedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public record SupportAgentTicketReleased(SupportAgentProfileId ProfileId, Guid AgentId, TicketId TicketId, Guid ReleasedBy) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
