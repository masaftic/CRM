using FluentValidation;
using SalesModule.Domain;
using Thinktecture;

namespace SalesModule.Api.Features.Pipelines.Validators;

public static class StrongIdValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValidPipelineId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must((value, context) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);
                return false;
            }

            var validationError = PipelineId.Validate(value, null, out var pipelineId);
            if (validationError != null)
            {
                context.MessageFormatter.AppendArgument("Error", validationError.Message);
                return false;
            }

            return true;
        })
        .WithMessage("'{PropertyName}' must be a valid PipelineId.");
    }

    public static IRuleBuilderOptions<T, string> MustBeValidStageId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must((value, context) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);
                return false;
            }

            var validationError = StageId.Validate(value, null, out var stageId);
            if (validationError != null)
            {
                context.MessageFormatter.AppendArgument("Error", validationError.Message);
                return false;
            }

            return true;
        })
        .WithMessage("'{PropertyName}' must be a valid StageId.");
    }
}