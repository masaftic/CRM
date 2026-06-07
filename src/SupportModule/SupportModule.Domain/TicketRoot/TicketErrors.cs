using BuildingBlocks.Domain;

namespace SupportModule.Domain.TicketRoot;

public static class TicketErrors
{
    public static readonly AppError ActorRequired = AppError.Validation(
        "Ticket.ActorRequired",
        "Actor id must be provided.");

    public static readonly AppError UrgentPriorityRequiresReason = AppError.Validation(
        "Ticket.Priority.UrgentReasonRequired",
        "Urgent priority changes require a reason.");

    public static AppError InvalidTransition(TicketStatus from, TicketStatus to) => AppError.Validation(
        "Ticket.Status.InvalidTransition",
        $"Cannot transition ticket from {from} to {to}.");
}
