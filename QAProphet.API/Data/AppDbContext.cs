using Microsoft.EntityFrameworkCore;
using QAProphet.API.Data.Models;

namespace QAProphet.API.Data;

public class AppDbContext : DbContext
{
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<AnswerComment> AnswerComments { get; set; }
    public DbSet<QuestionComment> QuestionComments { get; set; }
    public DbSet<QuestionTags> QuestionTags { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}