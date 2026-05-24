using Thinktecture;

namespace SalesModule.Domain;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    static partial void ValidateFactoryArguments(
        ref ValidationError? validationError,
        ref decimal amount,
        ref string currency)
    {
        if (amount < 0)
            validationError = new ValidationError("Amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            validationError = new ValidationError("Currency cannot be null or empty.");
        if (currency.Length != 3)
            validationError = new ValidationError("Currency must be a 3-letter ISO code.");
        
        currency = currency.ToUpperInvariant();
    }
}