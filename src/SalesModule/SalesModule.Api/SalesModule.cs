using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesModule.Api.Features.Pipelines;
using SalesModule.Infrastructure;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api;

public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Infrastructure(configuration);

        return services;
    }

    public static void MigrateSalesModuleDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
        dbContext.Database.Migrate();
    }

    public static WebApplication MapSalesModule(this WebApplication app)
    {
        var group = app.MapGroup("/pipelines").WithGroupName("Pipelines");

        group.MapPipelineEndpoints();

        return app;
    }
}
