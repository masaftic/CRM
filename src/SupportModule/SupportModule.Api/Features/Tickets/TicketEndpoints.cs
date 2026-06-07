using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;
using SupportModule.Contracts.Tickets.Requests;

namespace SupportModule.Api.Features.Tickets;

public static class TicketEndpoints
{
    public static RouteGroupBuilder MapTicketEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateTicket.Handle)
            .WithValidation<CreateTicketRequest>();
        routes.MapGet("/", GetTickets.Handle);
        routes.MapGet("{ticketId:guid}", GetTicket.Handle);
        routes.MapPost("{ticketId:guid}/assign", AssignTicket.Handle)
            .WithValidation<AssignTicketRequest>();
        routes.MapPost("{ticketId:guid}/resolve", ResolveTicket.Handle)
            .WithValidation<ResolveTicketRequest>();
        routes.MapPost("{ticketId:guid}/comments/customer", AddCustomerComment.Handle)
            .WithValidation<AddTicketCommentRequest>();
        routes.MapGet("{ticketId:guid}/comments", GetTicketComments.Handle);

        return routes;
    }
}
