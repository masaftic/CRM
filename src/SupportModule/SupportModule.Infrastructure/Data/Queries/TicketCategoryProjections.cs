using SupportModule.Contracts.TicketCategories.Responses;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Infrastructure.Data.Queries;

public static class TicketCategoryProjections
{
    public static IQueryable<TicketCategoryProjection> ToTicketCategoryProjection(this IQueryable<TicketCategory> query)
        => query.Select(category => new TicketCategoryProjection(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            category.DefaultPriority,
            category.RequiredSkills));

    public static TicketCategoryResponse ToResponse(this TicketCategoryProjection category)
        => new(
            category.CategoryId.ToString(),
            category.Name,
            category.Description,
            category.IsActive,
            category.DefaultPriority.ToString(),
            category.RequiredSkillIds.Select(skillId => skillId.ToString()).ToList());
}

public record TicketCategoryProjection(
    TicketCategoryId CategoryId,
    string Name,
    string Description,
    bool IsActive,
    TicketPriority DefaultPriority,
    IReadOnlyCollection<SkillId> RequiredSkillIds);
