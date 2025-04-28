using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class QuestionComplaintConfiguration : IEntityTypeConfiguration<QuestionComplaint>
{
    public void Configure(EntityTypeBuilder<QuestionComplaint> builder)
    {
        builder.HasKey(qc => qc.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);

        builder
            .HasOne(qc => qc.Question)
            .WithMany(q => q.Complaints)
            .HasForeignKey(qc => qc.QuestionId);
    }
}