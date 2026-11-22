using FlashCardApp.Attributes;
using FlashCardApp.Data.Configurations.Contracts;
using FlashCardApp.Models.Commons;
using FlashCardApp.Models.Entities;
using FlashCardApp.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashCardApp.Data.Schemas.Core;

public class PostgreSQLFlashCardSchemaConfiguration<TKey, TFlashCardSetKey, TOwnerKey, TStudySessionKey, TCardReviewKey> 
: IFlashCardModelConfiguration, IEntityTypeConfiguration<FlashCard<TKey, TFlashCardSetKey, TOwnerKey, TStudySessionKey, TCardReviewKey>>
{
    public void Apply(ModelBuilder builder)
    {
        builder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<FlashCard<TKey, TFlashCardSetKey, TOwnerKey, TStudySessionKey, TCardReviewKey>> builder)
    {
       builder.ToTable(AppConstants.Domains.Core.Tables.FlashCards, AppConstants.Domains.Core.Name);

       if (new[] { typeof(Guid), typeof(string) }.Contains(typeof(TFlashCardSetKey)))
        {
            builder.Property(x => x.SetId)
                .HasColumnName("set_id")
                .HasColumnType("UUID")
                .IsRequired();
        }
        else if (new[] { typeof(int), typeof(long) }.Contains(typeof(TFlashCardSetKey)))
        {
            builder.Property(x => x.SetId)
                .HasColumnName("set_id")
                .HasColumnType("BIGINT")
                .IsRequired();
        }
        else
        {
            throw new NotSupportedException($"The type {typeof(TFlashCardSetKey)} is not supported for the FlashCard SetId column.");
        }
        builder.HasOne(x => x.Set)
            .WithMany(x => x.FlashCards)
            .HasForeignKey(x => x.SetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Term)
            .IsRequired()
            .HasColumnName("term")
            .HasColumnType("TEXT");

        builder.ToTable(t => t.HasCheckConstraint("CK_FlashCard_Term_Length_GreaterThanZero", "length(btrim(term)) > 0"));        

        builder.Property(x => x.Definition)
            .IsRequired()
            .HasColumnName("definition")
            .HasColumnType("TEXT");

        builder.ToTable(t => t.HasCheckConstraint("CK_FlashCard_Definition_Length_GreaterThanZero", "length(btrim(definition)) > 0"));

        builder.Property(x => x.Position)
            .IsRequired()
            .HasColumnName("position")
            .HasColumnType("INT");

        builder.ToTable(t => t.HasCheckConstraint("CK_FlashCard_Position_GreaterThanZero", "position > 0"));
    }
}