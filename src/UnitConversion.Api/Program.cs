using Scalar.AspNetCore;
using UnitConversion.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IUnitConverter, UnitConverter>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // API docs: /openapi/v1.json and an interactive UI at /scalar
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();
