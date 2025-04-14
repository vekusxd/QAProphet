﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.API.Data.Models;

namespace QAProphet.API.Data.Configurations;

public class QuestionTagsConfiguration : IEntityTypeConfiguration<QuestionTags>
{
    public void Configure(EntityTypeBuilder<QuestionTags> builder)
    {
        builder.HasKey(questionTags => questionTags.Id);

        builder.HasOne(questionTags => questionTags.Tag)
            .WithMany(tag => tag.Questions)
            .HasForeignKey(q => q.TagId);

        builder.HasOne(questionTags => questionTags.Question)
            .WithMany(question => question.Tags)
            .HasForeignKey(questionTags => questionTags.QuestionId);
    }
}