using FluentValidation;
using SalesModule.Contracts.Pipelines.Requests;

namespace SalesModule.Api.Features.Pipelines.Validators;

public class GetPipelineValidator : AbstractValidator<GetPipelineRequest>
{
    public GetPipelineValidator()
    {
        RuleFor(x => x.PipelineId)
            .NotEmpty()
            .MustBeValidPipelineId();
    }
}