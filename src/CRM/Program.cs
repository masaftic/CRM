using SalesModule.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("sales", o =>
{

});
builder.Services.AddOpenApi("example", o =>
{

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSalesModule(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.AddDocument("sales", "/openapi/sales.json");
        options.AddDocument("example", "/openapi/example.json"); 
    });

    app.MigrateSalesModuleDatabase();
}

app.UseHttpsRedirection();

app.MapSalesModule();

app.MapGet("/api/hello", () => "Hello World!").WithGroupName("example");

app.Run();
