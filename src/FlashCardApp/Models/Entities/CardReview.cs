using FlashCardApp.Enums;
using FlashCardApp.Models.Commons;

namespace FlashCardApp.Models.Entities;

public class CardReview<TKey, TStudySessionKey, TFlashCardKey, TUserKey, TFlashCardSetKey> 
: Entity<TKey>
{
    public TStudySessionKey SessionId { get; set; } = default!;
    public TFlashCardKey CardId { get; set; } = default!;
    public ReviewResult Result { get; set; } = ReviewResult.Skipped;
    public int ResponseTimeMs { get; set; } = default!;
    public DateTime ReviewedAt { get; set; } = default!;

    // Navigation properties
    public StudySession<TStudySessionKey, TUserKey, TFlashCardSetKey, TFlashCardKey, TKey>? Session { get; set; }
    public FlashCard<TFlashCardKey, TFlashCardSetKey, TUserKey, TStudySessionKey, TKey>? Card { get; set; }
}