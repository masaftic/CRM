using SupportModule.Contracts.AgentProfiles.Responses;
using SupportModule.Contracts.Skills.Responses;
using SupportModule.Contracts.TicketCategories.Responses;
using SupportModule.Contracts.Tickets.Responses;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Domain.TicketCommentRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Api.Features;

public static class SupportResponseMappings
{
    public static SkillResponse ToResponse(this Skill skill)
        => new(skill.Id.ToString(), skill.Name, skill.IsActive);

    public static TicketCategoryResponse ToResponse(this TicketCategory category)
        => new(
            category.Id.ToString(),
            category.Name,
            category.Description,
            category.IsActive,
            category.DefaultPriority.ToString(),
            category.RequiredSkills.Select(x => x.ToString()).ToList());

    public static AgentProfileResponse ToResponse(this SupportAgentProfile profile)
        => new(
            profile.ProfileId.ToString(),
            profile.AgentId,
            profile.Name,
            profile.SkillIds.Select(x => x.ToString()).ToList(),
            profile.AvailabilityStatus.ToString(),
            profile.MaxActiveTickets,
            profile.CurrentActiveTickets,
            profile.IsActive,
            profile.ActiveTicketIds.Select(x => x.ToString()).ToList());

    public static TicketResponse ToResponse(this Ticket ticket)
        => new(
            ticket.TicketId.ToString(),
            ticket.CustomerId,
            ticket.CategoryId.ToString(),
            ticket.AssignedAgentId,
            ticket.Title,
            ticket.Description,
            ticket.Status.ToString(),
            ticket.Priority.ToString(),
            ticket.CreatedAtUtc,
            ticket.ResolvedAtUtc,
            ticket.ClosedAtUtc);

    public static TicketCommentResponse ToResponse(this TicketComment comment)
        => new(
            comment.Id.ToString(),
            comment.TicketId.ToString(),
            comment.Kind.ToString(),
            comment.CustomerId,
            comment.AgentId,
            comment.Content,
            comment.CreatedAtUtc);
}
