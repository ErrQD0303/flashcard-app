using FlashCardApp.Attributes;
using FlashCardApp.Data.Configurations.Contracts;
using FlashCardApp.Models.Commons;
using FlashCardApp.Models.Entities;
using FlashCardApp.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashCardApp.Data.Schemas.Core;

public class PostgreSQLUserSchemaConfiguration<TKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey> 
: IFlashCardModelConfiguration, IEntityTypeConfiguration<User<TKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey>>
{
    public void Apply(ModelBuilder builder)
    {
        builder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<User<TKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey>> builder)
    {
       builder.ToTable(AppConstants.Domains.Core.Tables.Users, AppConstants.Domains.Core.Name);

       builder.Property(x => x.Username)
           .HasColumnName("username")
           .HasColumnType("TEXT")
           .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_User_Username_Length_GreaterThanZero", "length(btrim(username)) > 0"));
        
        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_User_Email_Has_AtLeast_One_Characters_Before_At", "position('@' in email) > 1"));
    }
}