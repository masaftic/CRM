using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.TicketCategories.Requests;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.TicketCategories;

public static class AddRequiredSkill
{
    public static async Task<IResult> Handle(Guid categoryId, AddRequiredSkillRequest request, SupportDbContext db, CancellationToken ct)
    {
        var category = await db.TicketCategories.FirstOrDefaultAsync(x => x.Id == TicketCategoryId.Create(categoryId), ct);
        if (category is null)
            return AppError.NotFound("TicketCategory.NotFound", "Ticket category not found.").ToProblemDetails();

        var skillId = SkillId.Create(request.SkillId);
        var skillExists = await db.Skills.AnyAsync(x => x.Id == skillId, ct);
        if (!skillExists)
            return AppError.NotFound("Skill.NotFound", "Skill not found.").ToProblemDetails();

        category.RequireSkill(skillId);
        await db.SaveChangesAsync(ct);

        return Results.Ok(category.ToResponse());
    }
}
