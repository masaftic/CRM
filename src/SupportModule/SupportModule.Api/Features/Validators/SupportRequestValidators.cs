using FluentValidation;
using SupportModule.Contracts.AgentProfiles.Requests;
using SupportModule.Contracts.Skills.Requests;
using SupportModule.Contracts.TicketCategories.Requests;
using SupportModule.Contracts.Tickets.Requests;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Domain.TicketRoot;

namespace SupportModule.Api.Features.Validators;

public sealed class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>
{
    public CreateSkillRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateTicketCategoryRequestValidator : AbstractValidator<CreateTicketCategoryRequest>
{
    public CreateTicketCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DefaultPriority)
            .Must(value => Enum.TryParse<TicketPriority>(value, true, out _))
            .WithMessage("Default priority is invalid.");
    }
}

public sealed class AddRequiredSkillRequestValidator : AbstractValidator<AddRequiredSkillRequest>
{
    public AddRequiredSkillRequestValidator()
    {
        RuleFor(x => x.SkillId).NotEmpty();
    }
}

public sealed class CreateAgentProfileRequestValidator : AbstractValidator<CreateAgentProfileRequest>
{
    public CreateAgentProfileRequestValidator()
    {
        RuleFor(x => x.AgentId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AvailabilityStatus)
            .Must(value => Enum.TryParse<AgentAvailabilityStatus>(value, true, out _))
            .WithMessage("Availability status is invalid.");
        RuleFor(x => x.MaxActiveTickets).GreaterThan(0);
    }
}

public sealed class ChangeAgentAvailabilityRequestValidator : AbstractValidator<ChangeAgentAvailabilityRequest>
{
    public ChangeAgentAvailabilityRequestValidator()
    {
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.AvailabilityStatus)
            .Must(value => Enum.TryParse<AgentAvailabilityStatus>(value, true, out _))
            .WithMessage("Availability status is invalid.");
    }
}

public sealed class ChangeAgentCapacityRequestValidator : AbstractValidator<ChangeAgentCapacityRequest>
{
    public ChangeAgentCapacityRequestValidator()
    {
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.MaxActiveTickets).GreaterThan(0);
    }
}

public sealed class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}

public sealed class AssignTicketRequestValidator : AbstractValidator<AssignTicketRequest>
{
    public AssignTicketRequestValidator()
    {
        RuleFor(x => x.AgentId).NotEmpty();
        RuleFor(x => x.AssignedBy).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}

public sealed class ResolveTicketRequestValidator : AbstractValidator<ResolveTicketRequest>
{
    public ResolveTicketRequestValidator()
    {
        RuleFor(x => x.ResolvedBy).NotEmpty();
        RuleFor(x => x.Summary).MaximumLength(4000);
    }
}

public sealed class AddTicketCommentRequestValidator : AbstractValidator<AddTicketCommentRequest>
{
    public AddTicketCommentRequestValidator()
    {
        RuleFor(x => x.ActorId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}
