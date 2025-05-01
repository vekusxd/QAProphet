using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class QuestionComplaintCategoryConfiguration : IEntityTypeConfiguration<QuestionComplaintCategory>
{
    public void Configure(EntityTypeBuilder<QuestionComplaintCategory> builder)
    {
        builder.HasKey(qc => qc.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);
        
        builder
            .Property(qc => qc.Title)
            .HasMaxLength(96)
            .IsRequired();
        
        builder
            .HasMany(qc => qc.Complaints)
            .WithOne(c => c.Category)
            .HasForeignKey(qc => qc.CategoryId);
    }
}