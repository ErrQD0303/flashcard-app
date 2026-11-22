using Microsoft.EntityFrameworkCore;
using FlashCardApp.Data.Configurations;
using FlashCardApp.Data.Configurations.Contracts;
using FlashCardApp.Models.Entities;
using System.Reflection;
using FlashCardApp.Data.Schemas;
using FlashCardApp.Utils;
using FlashCardApp.Models.Commons;

namespace FlashCardApp.Data.Contracts;

public class AppCoreDbContext<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey> : DbContext
{
    #region Properties

    #endregion
    
    #region DbSets
    public DbSet<FlashCardSet<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey>> FlashCardSets { get; set; } 
    public DbSet<FlashCard<TFlashCardKey, TFlashCardSetKey, TUserKey, TStudySessionKey, TCardReviewKey>> FlashCards { get; set; }
    public DbSet<User<TUserKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey>> Users { get; set; }
    public DbSet<StudySession<TStudySessionKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>> StudySessions { get; set; }
    public DbSet<CardReview<TCardReviewKey, TStudySessionKey, TFlashCardKey, TUserKey, TFlashCardSetKey>> CardReviews { get; set; }

    #endregion

    #region Constructors
    public AppCoreDbContext(DbContextOptions<AppCoreDbContext<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey>> options)
        : base(options)
    {
    }
    #endregion

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyAppCoreConfigurations<TFlashCardSetKey, TFlashCardKey, TUserKey, TStudySessionKey, TCardReviewKey>();
    }
    #endregion

    #region Methods
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) 
    {
        StampAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = now;
                    break;
            }
        }
    }
    #endregion
}