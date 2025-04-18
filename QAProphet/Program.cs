using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QAProphet;
using QAProphet.Data;
using QAProphet.Extensions;
using QAProphet.Options;
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

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection(AuthOptions.Section));
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetRequiredSection(KeycloakOptions.Section));

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<Seed>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seed>();
    await seeder.SeedTags();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.Run();