namespace SalesModule.Contracts.Contacts.Events;

// should come from the contact/identity module, but not yet implemented
public record ContactCreatedIntegrationEvent(
    Guid ContactId,
    string Name,
    string CompanyName,
    DateTimeOffset CreatedAt);
