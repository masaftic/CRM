using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Data;

namespace Shared.Infrastructure.Data.Outbox;

public static class OutboxRegistrationExtensions
{
    public static IServiceCollection AddOutboxProcessor<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IOutboxDbContext, IUnitOfWork
    {
        services.AddHostedService<OutboxProcessor<TDbContext>>();

        return services;
    }
}
