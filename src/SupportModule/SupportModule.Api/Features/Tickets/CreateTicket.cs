using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Web.Extensions;
using SupportModule.Contracts.Tickets.Requests;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Domain.TicketRoot;
using SupportModule.Infrastructure.Data;
using SupportModule.Api.Features;

namespace SupportModule.Api.Features.Tickets;

public static class CreateTicket
{
    public static async Task<IResult> Handle(CreateTicketRequest request, SupportDbContext db, CancellationToken ct)
    {
        var category = await db.TicketCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == TicketCategoryId.Create(request.CategoryId), ct);

        if (category is null)
            return AppError.NotFound("TicketCategory.NotFound", "Ticket category not found.").ToProblemDetails();
        if (!category.IsActive)
            return AppError.Conflict("TicketCategory.Inactive", "Cannot create a ticket in an inactive category.").ToProblemDetails();

        var ticket = new Ticket(category.Id, request.CustomerId, request.Title, request.Description, category.DefaultPriority);

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/support/tickets/{ticket.TicketId}", ticket.ToResponse());
    }
}
