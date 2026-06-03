using FluentValidation;
using SalesModule.Api.Features.Pipelines.Validators;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;

namespace SalesModule.Api.Features.Deals.Validators;

public class ReopenDealRequestValidator : AbstractValidator<ReopenDealRequest>
{
    public ReopenDealRequestValidator()
    {
        RuleFor(x => x.StageId).MustBeValidStageId();
    }
}
