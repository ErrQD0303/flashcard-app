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

public class PostgreSQLFlashCardSetConfiguration<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey> 
: IFlashCardModelConfiguration, IEntityTypeConfiguration<FlashCardSet<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey>>
{
    public void Apply(ModelBuilder builder)
    {
        builder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<FlashCardSet<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey>> builder)
    {
        builder.ToTable(AppConstants.Domains.Core.Tables.FlashCardSets, AppConstants.Domains.Core.Name);

        builder.Property(x => x.OwnerId)
            .IsRequired()
            .HasColumnName("owner_id");
        
        if (typeof(TOwnerKey) == typeof(Guid) || typeof(TOwnerKey) == typeof(string) || typeof(TOwnerKey) == typeof(int) || typeof(TOwnerKey) == typeof(long))
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.FlashCardSets)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        else
        {
            throw new NotSupportedException($"The type {typeof(TOwnerKey)} is not supported for the FlashCardSet OwnerId column.");
        }

        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasColumnType("TEXT");
        
        builder.ToTable(t => t.HasCheckConstraint("CK_FlashCardSet_Title_Length_GreaterThanZero", "length(btrim(title)) > 0"));

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT");

        var defaultVisibility = typeof(FlashCardSetVisibility)
            .GetCustomAttributes<EnumMemberAttribute>()
            .FirstOrDefault(a => a.Value == "private")
            ?.Value;

        builder.Property(x => x.Visibility)
            .IsRequired()
            .HasColumnName("visibility")
            .HasColumnType("set_visibility")
            .HasDefaultValue(defaultVisibility);

         if (new[] { typeof(Guid), typeof(string) }.Contains(typeof(TKey)))
        {
            builder.Property(x => x.SourceSetId)
                .HasColumnName("source_set_id")
                .HasColumnType("UUID")
                .HasDefaultValue(null);
        }
        else if (new[] { typeof(int), typeof(long) }.Contains(typeof(TKey)))
        {
            builder.Property(x => x.SourceSetId)
                .HasColumnName("source_set_id")
                .HasColumnType("BIGINT")
                .HasDefaultValue(null);
        }
        else
        {
            throw new NotSupportedException($"The type {typeof(TKey)} is not supported for the FlashCardSet SourceSetId column.");
        }

        builder.HasOne(x => x.SourceSet)
            .WithMany(x => x.DerivedSets)
            .HasForeignKey(x => x.SourceSetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}