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

public class PostgreSQLStudySessionSchemaConfiguration<TKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>
: IFlashCardModelConfiguration, IEntityTypeConfiguration<StudySession<TKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>>
{
    public void Apply(ModelBuilder builder)
    {
        builder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<StudySession<TKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>> builder)
    {
        builder.ToTable(AppConstants.Domains.Core.Tables.StudySessions, AppConstants.Domains.Core.Name);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.StudySessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.FlashCardSetId)
            .HasColumnName("set_id")
            .IsRequired();

        builder.HasOne(x => x.FlashCardSet)
            .WithMany(x => x.StudySessions)
            .HasForeignKey(x => x.FlashCardSetId)
            .OnDelete(DeleteBehavior.Cascade);

        var defaultMode = typeof(StudyMode)
            .GetCustomAttributes<EnumMemberAttribute>()
            .FirstOrDefault(a => a.Value == "flashcards")
            ?.Value;

        builder.Property(x => x.Mode)
            .HasColumnName("mode")
            .IsRequired()
            .HasColumnType("study_mode")
            .HasDefaultValue(defaultMode);

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("TIMESTAMPTZ")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(x => x.EndedAt)
            .HasColumnName("ended_at")
            .HasColumnType("TIMESTAMPTZ")
            .IsRequired(false);

        builder.ToTable(t => t.HasCheckConstraint("CK_StudySession_StartedAt_Before_Or_Equal_To_EndedAt", "ended_at >= started_at"));
    }
}