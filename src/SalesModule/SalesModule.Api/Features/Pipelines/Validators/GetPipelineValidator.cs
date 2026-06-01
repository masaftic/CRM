using FluentValidation;
using SalesModule.Contracts.Pipelines.Requests;
using SalesModule.Api.Features.Pipelines.Validators;

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