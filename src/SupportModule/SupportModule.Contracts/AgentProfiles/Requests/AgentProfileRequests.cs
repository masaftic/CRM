namespace SupportModule.Contracts.AgentProfiles.Requests;

public record CreateAgentProfileRequest(
    Guid AgentId,
    string Name,
    List<Guid> SkillIds,
    string AvailabilityStatus,
    int MaxActiveTickets,
    bool IsActive);

public record ChangeAgentAvailabilityRequest(string AvailabilityStatus, Guid ChangedBy);

public record ChangeAgentCapacityRequest(int MaxActiveTickets, Guid ChangedBy);
