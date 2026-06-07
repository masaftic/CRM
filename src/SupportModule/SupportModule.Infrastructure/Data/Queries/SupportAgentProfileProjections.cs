using SupportModule.Contracts.AgentProfiles.Responses;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Infrastructure.Data.Queries;

public static class SupportAgentProfileProjections
{
    public static IQueryable<AgentProfileProjection> ToAgentProfileProjection(this IQueryable<SupportAgentProfile> query)
        => query.Select(profile => new AgentProfileProjection(
            profile.ProfileId,
            profile.AgentId,
            profile.Name,
            profile.SkillIds,
            profile.AvailabilityStatus,
            profile.MaxActiveTickets,
            profile.CurrentActiveTickets,
            profile.IsActive,
            profile.ActiveTicketIds));

    public static AgentProfileResponse ToResponse(this AgentProfileProjection profile)
        => new(
            profile.ProfileId.ToString(),
            profile.AgentId,
            profile.Name,
            profile.SkillIds.Select(skillId => skillId.ToString()).ToList(),
            profile.AvailabilityStatus.ToString(),
            profile.MaxActiveTickets,
            profile.CurrentActiveTickets,
            profile.IsActive,
            profile.ActiveTicketIds.Select(ticketId => ticketId.ToString()).ToList());
}

public record AgentProfileProjection(
    SupportAgentProfileId ProfileId,
    Guid AgentId,
    string Name,
    IReadOnlyCollection<SkillId> SkillIds,
    AgentAvailabilityStatus AvailabilityStatus,
    int MaxActiveTickets,
    int CurrentActiveTickets,
    bool IsActive,
    IReadOnlyCollection<TicketId> ActiveTicketIds);
