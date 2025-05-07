using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Extensions;
using QAProphet.Hubs;
using QAProphet.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string was not found");

builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));

builder.Services.Configure<AnswerTimeoutOptions>(
    builder.Configuration.GetRequiredSection(AnswerTimeoutOptions.Section));
builder.Services.Configure<QuestionTimeoutOptions>(
    builder.Configuration.GetRequiredSection(QuestionTimeoutOptions.Section));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(new KeycloakSecuritySchemeTransformer(builder.Configuration));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

builder.Services.AddSignalR();

builder.Services.Configure<AuthOptions>(builder.Configuration.GetRequiredSection(AuthOptions.Section));
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetRequiredSection(KeycloakOptions.Section));

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<Seed>();

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CorsPolicy",
        policyBuilder => policyBuilder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((_) => true)
            .AllowAnyHeader());

    opts.AddDefaultPolicy(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seed>();
    await seeder.SeedTags();
    await seeder.SeedComplaintCategories();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<TestHub>("/hubs/hub");

app.MapCarter();

app.Run();