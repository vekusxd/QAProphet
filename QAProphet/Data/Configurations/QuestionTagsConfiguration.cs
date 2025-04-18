using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class QuestionTagsConfiguration : IEntityTypeConfiguration<QuestionTags>
{
    public void Configure(EntityTypeBuilder<QuestionTags> builder)
    {
        builder.HasKey(questionTags => questionTags.Id);
        
        builder.HasQueryFilter(questionTags => !questionTags.IsDeleted);

        builder.HasOne(questionTags => questionTags.Tag)
            .WithMany(tag => tag.Questions)
            .HasForeignKey(q => q.TagId);

        builder.HasOne(questionTags => questionTags.Question)
            .WithMany(question => question.Tags)
            .HasForeignKey(questionTags => questionTags.QuestionId);
    }
}