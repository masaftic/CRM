using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Data.Outbox;
using Shared.Infrastructure.IntegrationEvents;
using Shared.Infrastructure.InternalMessaging;
using SupportModule.Infrastructure.Data;

namespace SupportModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        services.AddScoped<IDomainEventsDispatcher, MediatorDomainEventDispatcher>();
        services.AddScoped<DomainEventsInterceptor>();
        services.AddScoped<IOutboxWriter<SupportModuleMarker>, OutboxWriter<SupportDbContext, SupportModuleMarker>>();
        services.AddScoped<IIntegrationEventPublisher<SupportModuleMarker>, OutboxIntegrationEventPublisher<SupportModuleMarker>>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddDbContext<SupportDbContext>((sp, op) =>
        {
            op.UseNpgsql(configuration.GetConnectionString("Default"));
            op.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
        });

        return services;
    }
}
