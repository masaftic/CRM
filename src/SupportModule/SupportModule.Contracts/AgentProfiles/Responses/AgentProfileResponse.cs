namespace SupportModule.Contracts.AgentProfiles.Responses;

public record AgentProfileResponse(
    string ProfileId,
    Guid AgentId,
    string Name,
    IReadOnlyCollection<string> SkillIds,
    string AvailabilityStatus,
    int MaxActiveTickets,
    int CurrentActiveTickets,
    bool IsActive,
    IReadOnlyCollection<string> ActiveTicketIds);
