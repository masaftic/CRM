using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Data.Outbox;

public static class OutboxRegistrationExtensions
{
    public static IServiceCollection AddOutboxProcessor<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IOutboxDbContext
    {
        services.AddHostedService<OutboxProcessor<TDbContext>>();

        return services;
    }
}
