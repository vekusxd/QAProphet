using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class AnswerCommentConfiguration : IEntityTypeConfiguration<AnswerComment>
{
    public void Configure(EntityTypeBuilder<AnswerComment> builder)
    {
        builder.HasKey(answerComment => answerComment.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);
        
        builder.Property(answerComment => answerComment.AuthorName)
            .IsRequired()
            .HasMaxLength(96);

        builder.Property(answerComment => answerComment.Content)
            .IsRequired()
            .HasMaxLength(-1);
        
        builder.HasOne(answerComment => answerComment.Answer)
            .WithMany(answer => answer.Comments)
            .HasForeignKey(answerComment => answerComment.AnswerId);
        
    }
}