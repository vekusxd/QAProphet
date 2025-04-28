using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class TagSubscribeConfiguration : IEntityTypeConfiguration<TagSubscribe>
{
    public void Configure(EntityTypeBuilder<TagSubscribe> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);
        
        builder
            .HasOne(ts => ts.Tag)
            .WithMany(t => t.Subscribers)
            .HasForeignKey(t => t.TagId);
    }
}