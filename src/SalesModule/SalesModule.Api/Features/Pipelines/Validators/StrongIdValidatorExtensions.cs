using FluentValidation;
using SalesModule.Domain;

namespace SalesModule.Api.Features.Pipelines.Validators;

public static class StrongIdValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValidPipelineId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must((value) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            var validationError = PipelineId.Validate(value, null, out var pipelineId);
            if (validationError != null)
            {
                return false;
            }
            return true;
        })
        .WithMessage("'{PropertyName}' must be a valid PipelineId.");
    }

    public static IRuleBuilderOptions<T, string> MustBeValidStageId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must((value) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var validationError = StageId.Validate(value, null, out var stageId);
            if (validationError != null)
            {
                return false;
            }

            return true;
        })
        .WithMessage("'{PropertyName}' must be a valid StageId.");
    }
}