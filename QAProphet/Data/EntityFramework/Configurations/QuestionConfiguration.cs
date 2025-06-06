﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QAProphet.Domain;

namespace QAProphet.Data.EntityFramework.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(question => question.Id);
        
        builder.HasQueryFilter(questionComment => !questionComment.IsDeleted);

        builder
            .Property(question => question.Title)
            .IsRequired()
            .HasMaxLength(96);

        builder
            .Property(question => question.Content)
            .IsRequired()
            .HasMaxLength(-1);
        
        builder
            .Property(question => question.AuthorName)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .HasMany(question => question.Answers)
            .WithOne(answer => answer.Question)
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(question => question.Comments)
            .WithOne(questionComment => questionComment.Question)
            .HasForeignKey(questionComment => questionComment.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(question => question.Tags)
            .WithOne(questionTags => questionTags.Question)
            .HasForeignKey(questionTags => questionTags.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(question => question.Subscribers)
            .WithOne(subscriber => subscriber.Question)
            .HasForeignKey(question => question.QuestionId);
        
        builder
            .HasMany(question => question.Complaints)
            .WithOne(complaint => complaint.Question)
            .HasForeignKey(complaint => complaint.QuestionId);
    }
}