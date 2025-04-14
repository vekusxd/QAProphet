using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.API.Data.Models;

namespace QAProphet.API.Data.Configurations;

public class QuestionCommentConfiguration : IEntityTypeConfiguration<QuestionComment>
{
    public void Configure(EntityTypeBuilder<QuestionComment> builder)
    {
        builder.HasKey(questionComment => questionComment.Id);

        builder.Property(questionComment => questionComment.Content)
            .IsRequired()
            .HasMaxLength(-1);

        builder.HasOne(questionComment => questionComment.Question)
            .WithMany(question => question.Comments)
            .HasForeignKey(questionComment => questionComment.QuestionId);
    }
}