using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class AnswerComplaintConfiguration : IEntityTypeConfiguration<AnswerComplaint>
{
    public void Configure(EntityTypeBuilder<AnswerComplaint> builder)
    {
        builder.HasKey(ac => ac.Id);
        
        builder.HasQueryFilter(answerComment => !answerComment.IsDeleted);

        builder
            .HasOne(ac => ac.Answer)
            .WithMany(a => a.Complaints)
            .HasForeignKey(ac => ac.AnswerId);
    }
}