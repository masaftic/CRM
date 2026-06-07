using SalesModule.Api;
using SupportModule.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("sales", o =>
{

});
builder.Services.AddOpenApi("support", o =>
{

});
builder.Services.AddOpenApi("example", o =>
{

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSalesModule(builder.Configuration);
builder.Services.AddSupportModule(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.AddDocument("sales", "/openapi/sales.json");
        options.AddDocument("support", "/openapi/support.json");
        options.AddDocument("example", "/openapi/example.json"); 
    });

    app.MigrateSalesModuleDatabase();
    app.MigrateSupportModuleDatabase();
}

app.UseHttpsRedirection();

app.MapSalesModule();
app.MapSupportModule();

app.MapGet("/api/hello", () => "Hello World!").WithGroupName("example");

app.Run();
