using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.EntityFramework.Configurations;

public class QuestionSubscribeConfiguration : IEntityTypeConfiguration<QuestionSubscribe>
{
    public void Configure(EntityTypeBuilder<QuestionSubscribe> builder)
    {
        builder.HasKey(qs => qs.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);
        
        builder
            .HasOne(qs => qs.Question)
            .WithMany(q => q.Subscribers)
            .HasForeignKey(qs => qs.QuestionId);
    }
}