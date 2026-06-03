using FluentValidation;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Domain;
using SalesModule.Api.Features.Pipelines.Validators;

namespace SalesModule.Api.Features.Deals.Validators;

public class CreateDealRequestValidator : AbstractValidator<CreateDealRequest>
{
    public CreateDealRequestValidator()
    {
        RuleFor(x => x.ContactId).NotEmpty();
        RuleFor(x => x.SalesPersonId).NotEmpty();
        RuleFor(x => x.PipelineId).MustBeValidPipelineId();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ExpectedCloseDate).NotEmpty();
        RuleFor(x => x.Value).NotNull().SetValidator(new MoneyDtoValidator());
    }
}

public class MoneyDtoValidator : AbstractValidator<MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
