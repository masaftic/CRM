using SupportModule.Contracts.Skills.Responses;
using SupportModule.Domain.SkillRoot;

namespace SupportModule.Infrastructure.Data.Queries;

public static class SkillProjections
{
    public static IQueryable<SkillResponse> ToSkillResponses(this IQueryable<Skill> query)
        => query.Select(skill => new SkillResponse(
            skill.Id.ToString(),
            skill.Name,
            skill.IsActive));
}
