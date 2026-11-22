using FlashCardApp.Data.Schemas.Core;
using FlashCardApp.Models.Commons;
using Microsoft.EntityFrameworkCore;

namespace FlashCardApp.Data.Schemas;
public static class FlashCardAppCoreConfigurations
{
    public static void ApplyAppCoreConfigurations<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey>(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var baseType = entityType.ClrType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Entity<>))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasKey(nameof(Entity<>.Id));

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(Entity<>.Id))
                        .ValueGeneratedNever();
                    
                    var keyType = baseType.GetGenericArguments()[0];
                    if (new[] { typeof(int), typeof(long) }.Contains(keyType))
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(nameof(Entity<>.Id))
                            .HasColumnName("id")
                            .HasColumnType("BIGINT");
                    else if (new[] { typeof(Guid), typeof(string) }.Contains(keyType))
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(nameof(Entity<>.Id))
                            .HasColumnName("id")
                            .HasColumnType("UUID");
                    else
                        throw new NotSupportedException($"Key type '{keyType}' is not supported.");
                    break;
                }
                baseType = baseType.BaseType; // Move up the inheritance chain
            }

            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.CreatedAt))
                    .HasColumnName("created_at")
                    .HasColumnType("TIMESTAMPTZ")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.UpdatedAt))
                    .HasColumnName("updated_at")
                    .HasColumnType("TIMESTAMPTZ")
                    .HasDefaultValueSql("now()")
                    .IsRequired();
            }
        }

        modelBuilder.ApplyConfiguration(new PostgreSQLFlashCardSetConfiguration<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey>());
        modelBuilder.ApplyConfiguration(new PostgreSQLFlashCardSchemaConfiguration<TFlashCardKey, TFlashCardSetKey, TUserKey, TStudySessionKey, TCardReviewKey>());
        modelBuilder.ApplyConfiguration(new PostgreSQLUserSchemaConfiguration<TUserKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey>());
        modelBuilder.ApplyConfiguration(new PostgreSQLStudySessionSchemaConfiguration<TStudySessionKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>());
        modelBuilder.ApplyConfiguration(new PostgreSQLCardReviewSchemaConfiguration<TCardReviewKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey, TUserKey>());
    }
}