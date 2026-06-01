using FluentValidation;
using SalesModule.Contracts.Pipelines.Requests;

namespace SalesModule.Api.Features.Pipelines.Validators;

public class CreatePipelineRequestValidator : AbstractValidator<CreatePipelineRequest>
{
    public CreatePipelineRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pipeline name is required.")
            .MaximumLength(100).WithMessage("Pipeline name must not exceed 100 characters.");

        RuleForEach(x => x.Stages).SetValidator(new StageRequestValidator());
    }   

    public class StageRequestValidator : AbstractValidator<CreatePipelineRequest.CreateStageDto>
    {
        public StageRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Stage name is required.")
                .MaximumLength(100).WithMessage("Stage name must not exceed 100 characters.");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Stage order must be a non-negative integer.");

            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 100).WithMessage("Stage probability must be between 0 and 100.");
        }
    }
}

