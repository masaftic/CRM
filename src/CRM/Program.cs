using SalesModule.Domain;
using SalesModule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<SalesDbContext>(op =>
{
    op.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<IUnitOfWork, SalesDbContext>();
builder.Services.AddScoped<IRepository<Deal, DealId>>(provider => 
    provider.GetRequiredService<SalesDbContext>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

