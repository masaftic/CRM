using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Domain;
using SupportModule.Domain.Events;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.TicketRoot;
using Thinktecture;

namespace SupportModule.Domain.SupportAgentProfileRoot;


[ValueObject<Guid>]
public partial struct SupportAgentProfileId
{
    public static SupportAgentProfileId New() => Create(Guid.NewGuid());
}


public class SupportAgentProfile : AggregateRoot
{
    public required SupportAgentProfileId ProfileId { get; init; }
    public Guid AgentId { get; init; }
    public string Name { get; private set; } = null!;

    private readonly List<SkillId> _skillIds = [];
    public IReadOnlyCollection<SkillId> SkillIds => _skillIds.AsReadOnly();

    public AgentAvailabilityStatus AvailabilityStatus { get; private set; }
    public int MaxActiveTickets { get; private set; }
    public int CurrentActiveTickets => _activeTicketIds.Count;
    public bool IsActive { get; private set; }
    public bool CanAcceptTicket =>
        IsActive &&
        AvailabilityStatus == AgentAvailabilityStatus.Available &&
        CurrentActiveTickets < MaxActiveTickets;

    private readonly List<TicketId> _activeTicketIds = [];
    public IReadOnlyCollection<TicketId> ActiveTicketIds => _activeTicketIds.AsReadOnly();

    private SupportAgentProfile() { }

    [SetsRequiredMembers]
    public SupportAgentProfile(
        Guid agentId,
        string name,
        List<SkillId> skills,
        AgentAvailabilityStatus availabilityStatus,
        int maxActiveTickets,
        bool isActive)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("AgentId must be provided.", nameof(agentId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be provided.", nameof(name));
        if (maxActiveTickets <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxActiveTickets), "Max active tickets must be greater than zero.");

        ProfileId = SupportAgentProfileId.New();
        AgentId = agentId;
        Name = name.Trim();
        _skillIds.AddRange(skills.Distinct());
        AvailabilityStatus = availabilityStatus;
        MaxActiveTickets = maxActiveTickets;
        IsActive = isActive;

        RaiseDomainEvent(new SupportAgentProfileCreated(ProfileId, AgentId, Name));
    }

    public Result Rename(string name, Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be provided.", nameof(name));

        var trimmedName = name.Trim();
        if (Name == trimmedName) return Result.Ok();

        Name = trimmedName;

        RaiseDomainEvent(new SupportAgentProfileRenamed(ProfileId, AgentId, Name, changedBy));

        return Result.Ok();
    }

    public Result Activate(Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (IsActive) return Result.Ok();

        IsActive = true;

        RaiseDomainEvent(new SupportAgentProfileActivated(ProfileId, AgentId, changedBy));

        return Result.Ok();
    }

    public Result Deactivate(Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (!IsActive) return Result.Ok();
        if (CurrentActiveTickets > 0)
            return SupportAgentProfileErrors.HasActiveTickets;

        IsActive = false;

        RaiseDomainEvent(new SupportAgentProfileDeactivated(ProfileId, AgentId, changedBy));

        return Result.Ok();
    }

    public Result ChangeAvailability(AgentAvailabilityStatus availabilityStatus, Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (AvailabilityStatus == availabilityStatus) return Result.Ok();

        var previousAvailabilityStatus = AvailabilityStatus;
        AvailabilityStatus = availabilityStatus;

        RaiseDomainEvent(new SupportAgentAvailabilityChanged(ProfileId, AgentId, previousAvailabilityStatus, availabilityStatus, changedBy));

        return Result.Ok();
    }

    public Result ChangeCapacity(int maxActiveTickets, Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (maxActiveTickets <= 0)
            return SupportAgentProfileErrors.InvalidCapacity;
        if (maxActiveTickets < CurrentActiveTickets)
            return SupportAgentProfileErrors.CapacityBelowCurrentLoad;
        if (MaxActiveTickets == maxActiveTickets) return Result.Ok();

        var previousMaxActiveTickets = MaxActiveTickets;
        MaxActiveTickets = maxActiveTickets;

        RaiseDomainEvent(new SupportAgentCapacityChanged(ProfileId, AgentId, previousMaxActiveTickets, maxActiveTickets, changedBy));

        return Result.Ok();
    }

    public Result AddSkill(SkillId skillId, Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (_skillIds.Contains(skillId)) return Result.Ok();

        _skillIds.Add(skillId);

        RaiseDomainEvent(new SupportAgentSkillAdded(ProfileId, AgentId, skillId, changedBy));

        return Result.Ok();
    }

    public Result RemoveSkill(SkillId skillId, Guid changedBy)
    {
        if (changedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (!_skillIds.Remove(skillId)) return Result.Ok();

        RaiseDomainEvent(new SupportAgentSkillRemoved(ProfileId, AgentId, skillId, changedBy));

        return Result.Ok();
    }

    public Result AssignTicket(TicketId ticketId, Guid assignedBy)
    {
        if (assignedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (_activeTicketIds.Contains(ticketId)) return Result.Ok();
        if (!CanAcceptTicket)
            return SupportAgentProfileErrors.CannotAcceptTicket;

        _activeTicketIds.Add(ticketId);

        RaiseDomainEvent(new SupportAgentTicketAssigned(ProfileId, AgentId, ticketId, assignedBy));

        return Result.Ok();
    }

    public Result ReleaseTicket(TicketId ticketId, Guid releasedBy)
    {
        if (releasedBy == Guid.Empty)
            return SupportAgentProfileErrors.ActorRequired;
        if (!_activeTicketIds.Remove(ticketId)) return Result.Ok();

        RaiseDomainEvent(new SupportAgentTicketReleased(ProfileId, AgentId, ticketId, releasedBy));

        return Result.Ok();
    }
}
