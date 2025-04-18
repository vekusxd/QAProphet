using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(question => question.Id);

        builder.Property(question => question.Title)
            .IsRequired()
            .HasMaxLength(96);

        builder.Property(question => question.Content)
            .IsRequired()
            .HasMaxLength(-1);

        builder.HasMany(question => question.Answers)
            .WithOne(answer => answer.Question)
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(question => question.Comments)
            .WithOne(questionComment => questionComment.Question)
            .HasForeignKey(questionComment => questionComment.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(question => question.Tags)
            .WithOne(questionTags => questionTags.Question)
            .HasForeignKey(questionTags => questionTags.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}