using SalesModule.Contracts.Contacts.Events;
using SalesModule.Infrastructure.Data;
using SalesModule.Infrastructure.Data.ReadModels;
using Shared.Infrastructure.IntegrationEvents;

namespace SalesModule.Infrastructure.IntegrationEventHandlers;

public sealed class CacheContactWhenCreated(SalesDbContext dbContext)
    : IIntegrationEventHandler<ContactCreatedIntegrationEvent>
{
    public async Task Handle(ContactCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var contact = await dbContext.Contacts
            .FindAsync([integrationEvent.ContactId], cancellationToken);

        if (contact is null)
        {
            dbContext.Contacts.Add(new SalesContact
            {
                ContactId = integrationEvent.ContactId,
                Name = integrationEvent.Name,
                CompanyName = integrationEvent.CompanyName,
                CreatedAt = integrationEvent.CreatedAt
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        contact.Name = integrationEvent.Name;
        contact.CompanyName = integrationEvent.CompanyName;
        contact.CreatedAt = integrationEvent.CreatedAt;
        contact.SyncedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
