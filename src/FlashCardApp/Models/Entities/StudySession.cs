using FlashCardApp.Enums;
using FlashCardApp.Models.Commons;

namespace FlashCardApp.Models.Entities;

public class StudySession<TKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey> : Entity<TKey>
{
    public TUserKey UserId { get; set; } = default!;
    public TFlashCardSetKey FlashCardSetId { get; set; } = default!;
    public StudyMode Mode { get; set; } = StudyMode.Flashcards;
    public DateTime StartedAt { get; set; } = default!;
    public DateTime? EndedAt { get; set; } = default!;

    // Navigation properties
    public User<TUserKey, TFlashCardSetKey, TFlashCardKey, TKey, TCardReviewKey> User { get; set; } = default!;
    public FlashCardSet<TFlashCardSetKey, TFlashCardKey, TUserKey, TKey, TCardReviewKey> FlashCardSet { get; set; } = default!;
    public ICollection<CardReview<TCardReviewKey, TKey, TFlashCardKey, TUserKey, TFlashCardSetKey>> CardReviews { get; set; } = default!;
}
