using SalesModule.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSalesModule(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MigrateSalesModuleDatabase();
}

app.UseHttpsRedirection();

app.MapSalesModule();

app.Run();
