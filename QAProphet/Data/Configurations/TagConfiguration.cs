using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(tag => tag.Id);

        builder.HasQueryFilter(tagComment => !tagComment.IsDeleted);

        builder
            .Property(tag => tag.Title)
            .IsRequired()
            .HasMaxLength(96);

        builder
            .HasMany(tag => tag.Questions)
            .WithOne(questionTags => questionTags.Tag)
            .HasForeignKey(questionTags => questionTags.TagId);

        builder
            .Property(tag => tag.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder
            .HasMany(tag => tag.Subscribers)
            .WithOne(subscriberTags => subscriberTags.Tag)
            .HasForeignKey(subscriberTags => subscriberTags.TagId);
    }
}