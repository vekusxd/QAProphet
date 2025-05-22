using Microsoft.EntityFrameworkCore;
using QAProphet.Domain;

namespace QAProphet.Data.EntityFramework;

public class AppDbContext : DbContext
{
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<AnswerComment> AnswerComments { get; set; }
    public DbSet<QuestionComment> QuestionComments { get; set; }
    public DbSet<QuestionTags> QuestionTags { get; set; }
    public DbSet<AnswerLike> AnswerLikes { get; set; }
    public DbSet<TagSubscribe> TagSubscribes { get; set; }
    public DbSet<QuestionSubscribe> QuestionSubscribes { get; set; }
    public DbSet<QuestionComplaint> QuestionComplaints { get; set; }
    public DbSet<AnswerComplaint> AnswerComplaints { get; set; }
    public DbSet<QuestionComplaintCategory> QuestionComplaintCategories { get; set; }
    public DbSet<AnswerComplaintCategory> AnswerComplaintCategories { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}