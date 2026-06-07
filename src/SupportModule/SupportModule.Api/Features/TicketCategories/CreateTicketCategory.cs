using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.TicketCategories.Requests;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.TicketCategories;

public static class CreateTicketCategory
{
    public static async Task<IResult> Handle(CreateTicketCategoryRequest request, SupportDbContext db, CancellationToken ct)
    {
        var priority = Enum.Parse<TicketPriority>(request.DefaultPriority, true);

        var skillIds = request.RequiredSkillIds.Select(SkillId.Create).Distinct().ToList();
        var existingSkillIds = await db.Skills
            .Where(x => skillIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingSkillIds.Count != skillIds.Count)
            return AppError.Validation("TicketCategory.RequiredSkillMissing", "One or more required skills do not exist.").ToProblemDetails();

        var category = new TicketCategory(request.Name, request.Description, isActive: true, priority, skillIds);

        db.TicketCategories.Add(category);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/support/ticket-categories/{category.Id}", category.ToResponse());
    }
}
