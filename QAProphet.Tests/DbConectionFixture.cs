using Microsoft.EntityFrameworkCore;
using Npgsql;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Tests;
using Respawn;
using Testcontainers.PostgreSql;


[assembly: AssemblyFixture(typeof(DbConnectionFixture))]

namespace QAProphet.Tests;

public sealed class DbConnectionFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    private string _connectionString = null!;
    private readonly List<Guid> _tagIds = [];
    public IReadOnlyCollection<Guid> TagIds => _tagIds;
    public AppDbContext DbContext { get; private set; } = null!;
    private Respawner _respawner = null!;

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_connectionString).Options;
        DbContext = new AppDbContext(options);

        await DbContext.Database.EnsureCreatedAsync();

        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            conn,
            new RespawnerOptions { SchemasToInclude = ["public", "postgres"], DbAdapter = DbAdapter.Postgres }
        );
        await _respawner.ResetAsync(conn);

        conn.Dispose();
    }

    public async Task SeedAsync()
    {
        for (var i = 0; i < 5; i++)
        {
            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Description = $"Tag #{i} description",
                CreatedAt = DateTime.UtcNow,
                Title = $"Tag #{i} title"
            };
            _tagIds.Add(tag.Id);
            DbContext.Add(tag);
        }

        await DbContext.SaveChangesAsync();
    }

    public async Task ResetAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await _respawner.ResetAsync(conn);
        _tagIds.Clear();
    }
}