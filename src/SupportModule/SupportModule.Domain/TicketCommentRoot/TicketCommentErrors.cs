using BuildingBlocks.Domain;

namespace SupportModule.Domain.TicketCommentRoot;

public static class TicketCommentErrors
{
    public static readonly AppError ActorRequired = AppError.Validation(
        "TicketComment.ActorRequired",
        "Actor id must be provided.");

    public static readonly AppError ContentRequired = AppError.Validation(
        "TicketComment.ContentRequired",
        "Content must be provided.");
}
