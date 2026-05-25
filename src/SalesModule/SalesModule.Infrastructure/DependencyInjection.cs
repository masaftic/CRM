using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesModule.Domain;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection Infrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SalesDbContext>(op =>
        {
            op.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<IUnitOfWork, SalesDbContext>();
        services.AddScoped<IRepository<Deal, DealId>>(provider =>
            provider.GetRequiredService<SalesDbContext>());

        return services;
    }
}
