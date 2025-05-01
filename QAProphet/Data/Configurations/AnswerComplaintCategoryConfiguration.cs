using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class AnswerComplaintCategoryConfiguration : IEntityTypeConfiguration<AnswerComplaintCategory>
{
    public void Configure(EntityTypeBuilder<AnswerComplaintCategory> builder)
    {
        builder.HasKey(ac => ac.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);
        
        builder
            .Property(ac => ac.Title)
            .IsRequired()
            .HasMaxLength(96);
        
        builder
            .HasMany(ac => ac.Complaints)
            .WithOne(ac => ac.Category)
            .HasForeignKey(ac => ac.CategoryId);
    }
}