using System.Reflection;
using System.Runtime.Serialization;
using FlashCardApp.Attributes;
using FlashCardApp.Data.Configurations.Contracts;
using FlashCardApp.Enums;
using FlashCardApp.Models.Commons;
using FlashCardApp.Models.Entities;
using FlashCardApp.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashCardApp.Data.Schemas.Core;

public class PostgreSQLCardReviewSchemaConfiguration<TKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey, TUserKey> 
: IFlashCardModelConfiguration, IEntityTypeConfiguration<CardReview<TKey, TStudySessionKey, TFlashCardKey, TUserKey, TFlashCardSetKey>>
{
    public void Apply(ModelBuilder builder)
    {
        builder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<CardReview<TKey, TStudySessionKey, TFlashCardKey, TUserKey, TFlashCardSetKey>> builder)
    {
       builder.ToTable(AppConstants.Domains.Core.Tables.CardReviews, AppConstants.Domains.Core.Name);

       builder.Property(x => x.SessionId)
           .HasColumnName("session_id");

        if (typeof(TStudySessionKey) == typeof(int) || typeof(TStudySessionKey) == typeof(long))
            builder.Property(x => x.SessionId)
                .HasColumnType("BIGINT");
        else if (typeof(TStudySessionKey) == typeof(Guid) || typeof(TStudySessionKey) == typeof(string))
            builder.Property(x => x.SessionId)
                .HasColumnType("UUID");
        else
            throw new NotSupportedException($"Key type '{typeof(TStudySessionKey)}' is not supported.");

        builder.HasOne(x => x.Session)
            .WithMany(x => x.CardReviews)
            .HasForeignKey(x => x.SessionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CardId)
            .HasColumnName("card_id");

        if (typeof(TFlashCardKey) == typeof(int) || typeof(TFlashCardKey) == typeof(long))
            builder.Property(x => x.CardId)
                .HasColumnType("BIGINT");
        else if (typeof(TFlashCardKey) == typeof(Guid) || typeof(TFlashCardKey) == typeof(string))
            builder.Property(x => x.CardId)
                .HasColumnType("UUID");
        else
            throw new NotSupportedException($"Key type '{typeof(TFlashCardKey)}' is not supported.");

        builder.HasOne(x => x.Card)
            .WithMany(x => x.CardReviews)
            .HasForeignKey(x => x.CardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

       var defaultReviewResult = typeof(ReviewResult)
            .GetCustomAttributes<EnumMemberAttribute>()
            .FirstOrDefault(a => a.Value == "skipped")
            ?.Value;

        builder.Property(x => x.Result)
            .HasColumnName("result")
            .HasColumnType("review_result")
            .HasDefaultValue(defaultReviewResult)
            .IsRequired();

        builder.Property(x => x.ResponseTimeMs)
            .HasColumnName("response_time_ms")
            .HasColumnType("INT");

        builder.ToTable(t => t.HasCheckConstraint("CK_CardReview_ResponseTimeMs_NonNegative", "response_time_ms >= 0"));

        builder.Property(x => x.ReviewedAt)
            .HasColumnName("reviewed_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}