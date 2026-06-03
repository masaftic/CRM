namespace SalesModule.Contracts.Contacts.Events;

public record ContactCreatedIntegrationEvent(
    Guid ContactId,
    string Name,
    string CompanyName,
    DateTimeOffset CreatedAt);
