using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportModule.Api.Features.AgentProfiles;
using SupportModule.Api.Features.Skills;
using SupportModule.Api.Features.TicketCategories;
using SupportModule.Api.Features.Tickets;
using SupportModule.Api.Features.Validators;
using SupportModule.Infrastructure;
using SupportModule.Infrastructure.Data;

namespace SupportModule.Api;

public static class SupportModule
{
    public static IServiceCollection AddSupportModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSupportInfrastructure(configuration);
        services.AddValidatorsFromAssemblyContaining<CreateSkillRequestValidator>();

        return services;
    }

    public static void MigrateSupportModuleDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SupportDbContext>();
        dbContext.Database.Migrate();
    }

    public static WebApplication MapSupportModule(this WebApplication app)
    {
        var supportGroup = app.MapGroup("/support").WithGroupName("support");

        supportGroup.MapGroup("/skills")
            .WithTags("support - skills")
            .MapSkillEndpoints();

        supportGroup.MapGroup("/ticket-categories")
            .WithTags("support - ticket categories")
            .MapTicketCategoryEndpoints();

        supportGroup.MapGroup("/agent-profiles")
            .WithTags("support - agent profiles")
            .MapAgentProfileEndpoints();

        supportGroup.MapGroup("/tickets")
            .WithTags("support - tickets")
            .MapTicketEndpoints();

        return app;
    }
}
