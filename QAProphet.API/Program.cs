using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QAProphet.API.Data;
using QAProphet.API.Extensions;
using QAProphet.API.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string was not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(new KeycloakSecuritySchemeTransformer(builder.Configuration));
});

builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection(AuthOptions.Section));
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetRequiredSection(KeycloakOptions.Section));

builder.Services.AddAuth(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .RequireAuthorization();

app.MapGet("/me", (ClaimsPrincipal user) => 
    user.Claims.ToDictionary(c => c.Type, c => c.Value))
    .WithName("GetMe")
    .RequireAuthorization();

app.MapGet("/my-id", (ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)!.Value)
    .WithName("GetMyId")
    .RequireAuthorization();

app.Run();