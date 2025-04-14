using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.API.Data.Models;

namespace QAProphet.API.Data.Configurations;

public class AnswerCommentConfiguration : IEntityTypeConfiguration<AnswerComment>
{
    public void Configure(EntityTypeBuilder<AnswerComment> builder)
    {
        builder.HasKey(answerComment => answerComment.Id);

        builder.Property(answerComment => answerComment.Content)
            .IsRequired()
            .HasMaxLength(-1);
        
        builder.HasOne(answerComment => answerComment.Answer)
            .WithMany(answer => answer.Comments)
            .HasForeignKey(answerComment => answerComment.AnswerId);
    }
}