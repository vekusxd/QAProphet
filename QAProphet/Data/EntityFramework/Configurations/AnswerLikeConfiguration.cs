using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.EntityFramework.Configurations;

public class AnswerLikeConfiguration : IEntityTypeConfiguration<AnswerLike>
{
    public void Configure(EntityTypeBuilder<AnswerLike> builder)
    {
        builder.HasKey(answerLike => answerLike.Id);
        
        builder.HasQueryFilter(answerLike => !answerLike.IsDeleted);
        
        builder
            .HasOne(answerLike => answerLike.Answer)
            .WithMany(answer => answer.AnswerLikes)
            .HasForeignKey(answerLike => answerLike.AnswerId);
    }
}