using FluentValidation;
using SalesModule.Api.Features.Pipelines.Validators;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;

namespace SalesModule.Api.Features.Deals.Validators;

public class TransitionDealRequestValidator : AbstractValidator<TransitionDealRequest>
{
    public TransitionDealRequestValidator()
    {
        RuleFor(x => x.StageId).MustBeValidStageId();
    }
}
