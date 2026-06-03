using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesModule.Contracts.Contacts.Events;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using SalesModule.Infrastructure.Data.Queries;
using SalesModule.Infrastructure.IntegrationEventHandlers;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Data.Outbox;
using Shared.Infrastructure.IntegrationEvents;
using Shared.Infrastructure.InternalMessaging;

namespace SalesModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection Infrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        services.AddScoped<IDomainEventsDispatcher, MediatorDomainEventDispatcher>();
        services.AddScoped<DomainEventsInterceptor>();
        services.AddScoped<IOutboxWriter<SalesModuleMarker>, OutboxWriter<SalesDbContext, SalesModuleMarker>>();
        services.AddScoped<IIntegrationEventPublisher<SalesModuleMarker>, OutboxIntegrationEventPublisher<SalesModuleMarker>>();
        services.AddScoped<IIntegrationEventHandler<ContactCreatedIntegrationEvent>, CacheContactWhenCreated>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddDbContext<SalesDbContext>((sp, op) =>
        {
            op.UseNpgsql(configuration.GetConnectionString("Default"));
            op.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
        });

        services.AddScoped<ISalesUnitOfWork, SalesDbContext>();
        services.AddScoped<IRepository<Deal, DealId>>(provider =>
            provider.GetRequiredService<SalesDbContext>());
        
        services.AddScoped<IRepository<Pipeline, PipelineId>>(provider =>
            provider.GetRequiredService<SalesDbContext>());
        
        services.AddScoped<IDealsQueries, DealsQueries>();

        return services;
    }
}
