using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(answer => answer.Id);

        builder.HasQueryFilter(answer => !answer.IsDeleted);

        builder
            .Property(answer => answer.AuthorName)
            .IsRequired()
            .HasMaxLength(96);

        builder
            .Property(answer => answer.Content)
            .IsRequired()
            .HasMaxLength(-1);

        builder
            .HasOne(answer => answer.Question)
            .WithMany(question => question.Answers)
            .HasForeignKey(answer => answer.QuestionId);

        builder
            .HasMany(answer => answer.Comments)
            .WithOne(answerComment => answerComment.Answer)
            .HasForeignKey(answerComment => answerComment.AnswerId);
        
        builder
            .HasMany(answer => answer.AnswerLikes)
            .WithOne(answerLike => answerLike.Answer)
            .HasForeignKey(answerLike => answerLike.AnswerId);
        
        builder
            .HasMany(answer => answer.Complaints)
            .WithOne(complaint => complaint.Answer)
            .HasForeignKey(complaint => complaint.AnswerId);
    }
}

