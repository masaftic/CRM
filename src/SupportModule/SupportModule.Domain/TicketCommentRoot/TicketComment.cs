using BuildingBlocks.Domain;
using SupportModule.Domain.Events;
using SupportModule.Domain.TicketRoot;
using System.Diagnostics.CodeAnalysis;
using Thinktecture;

namespace SupportModule.Domain.TicketCommentRoot;

[ValueObject<Ulid>]
public partial struct TicketCommentId
{
    public static TicketCommentId New() => Create(Ulid.NewUlid());
}

public class TicketComment : AggregateRoot
{
    public TicketCommentId Id { get; init; } = TicketCommentId.New();
    public required TicketId TicketId { get; init; }
    public TicketCommentKind Kind { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid? AgentId { get; init; }
    public string Content { get; init; } = null!;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    private TicketComment() { }

    [SetsRequiredMembers]
    private TicketComment(TicketId ticketId, TicketCommentKind kind, Guid? customerId, Guid? agentId, string content)
    {
        TicketId = ticketId;
        Kind = kind;
        CustomerId = customerId;
        AgentId = agentId;
        Content = content.Trim();
    }

    public static Result<TicketComment> CustomerReply(TicketId ticketId, Guid customerId, string content)
    {
        var validation = Validate(customerId, content);
        if (validation.IsError) return validation.Errors;

        var comment = new TicketComment(ticketId, TicketCommentKind.CustomerReply, customerId, null, content);
        comment.RaiseDomainEvent(new CustomerRepliedToTicket(comment.Id, ticketId, customerId));

        return comment;
    }

    public static Result<TicketComment> AgentReply(TicketId ticketId, Guid agentId, string content)
    {
        var validation = Validate(agentId, content);
        if (validation.IsError) return validation.Errors;

        var comment = new TicketComment(ticketId, TicketCommentKind.AgentReply, null, agentId, content);
        comment.RaiseDomainEvent(new AgentRepliedToTicket(comment.Id, ticketId, agentId));

        return comment;
    }

    public static Result<TicketComment> InternalNote(TicketId ticketId, Guid agentId, string content)
    {
        var validation = Validate(agentId, content);
        if (validation.IsError) return validation.Errors;

        var comment = new TicketComment(ticketId, TicketCommentKind.InternalNote, null, agentId, content);
        comment.RaiseDomainEvent(new InternalTicketNoteAdded(comment.Id, ticketId, agentId));

        return comment;
    }

    private static Result Validate(Guid actorId, string content)
    {
        if (actorId == Guid.Empty)
            return TicketCommentErrors.ActorRequired;
        if (string.IsNullOrWhiteSpace(content))
            return TicketCommentErrors.ContentRequired;

        return Result.Ok();
    }
}

public enum TicketCommentKind
{
    CustomerReply,
    AgentReply,
    InternalNote
}
