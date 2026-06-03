using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesModule.Api.Features.Deals;
using SalesModule.Api.Features.Pipelines;
using SalesModule.Api.Features.Pipelines.Validators;
using SalesModule.Infrastructure;
using SalesModule.Infrastructure.Data;

namespace SalesModule.Api;

public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Infrastructure(configuration);
        services.AddValidatorsFromAssemblyContaining<CreatePipelineRequestValidator>();

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
        var salesGroup = app.MapGroup("/sales").WithGroupName("sales");

        var pipelinesGroup = salesGroup.MapGroup("/pipelines").WithTags("sales - pipelines");
        pipelinesGroup.MapPipelineEndpoints();

        var dealsGroup = salesGroup.MapGroup("/deals").WithTags("sales - deals");
        dealsGroup.MapDealEndpoints();

        return app;
    }
}
