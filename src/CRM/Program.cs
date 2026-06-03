using SalesModule.Api;

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
    app.MapSwaggerUI(setupAction: options =>
    {
        options.SwaggerEndpoint("/openapi/sales.json", "sales");
        options.SwaggerEndpoint("/openapi/example.json", "example");
    });

    app.MigrateSalesModuleDatabase();
}

app.UseHttpsRedirection();

app.MapSalesModule();

app.MapGet("/api/hello", () => "Hello World!").WithGroupName("example");

app.Run();
