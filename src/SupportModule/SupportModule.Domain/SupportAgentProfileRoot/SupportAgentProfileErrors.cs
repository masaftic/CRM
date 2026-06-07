using BuildingBlocks.Domain;

namespace SupportModule.Domain.SupportAgentProfileRoot;

public static class SupportAgentProfileErrors
{
    public static readonly AppError ActorRequired = AppError.Validation(
        "SupportAgentProfile.ActorRequired",
        "Actor id must be provided.");

    public static readonly AppError HasActiveTickets = AppError.Conflict(
        "SupportAgentProfile.HasActiveTickets",
        "Cannot deactivate an agent profile with active tickets.");

    public static readonly AppError InvalidCapacity = AppError.Validation(
        "SupportAgentProfile.Capacity.Invalid",
        "Max active tickets must be greater than zero.");

    public static readonly AppError CapacityBelowCurrentLoad = AppError.Validation(
        "SupportAgentProfile.Capacity.BelowCurrentLoad",
        "Max active tickets cannot be lower than the current active ticket count.");

    public static readonly AppError CannotAcceptTicket = AppError.Conflict(
        "SupportAgentProfile.CannotAcceptTicket",
        "Agent profile cannot accept more tickets.");
}
