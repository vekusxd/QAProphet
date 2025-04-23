using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;

namespace QAProphet.Tests;

internal sealed class DbContextWrapper : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private List<Guid> _tagIds = [];
    private SqliteConnection _connection;
    
    public AppDbContext DbContext  => new AppDbContext(_options);
    public IReadOnlyCollection<Guid> TagIds => _tagIds;
    
    public DbContextWrapper()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
       using var dbContext = new AppDbContext(_options);
       dbContext.Database.EnsureCreated();
       
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
           dbContext.Add(tag);
       }
       dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}