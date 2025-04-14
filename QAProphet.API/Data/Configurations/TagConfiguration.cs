using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.API.Data.Models;

namespace QAProphet.API.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Title)
            .IsRequired()
            .HasMaxLength(96);

        builder.HasMany(tag => tag.Questions)
            .WithOne(questionTags => questionTags.Tag)
            .HasForeignKey(questionTags => questionTags.TagId);
    }
}