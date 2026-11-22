using FlashCardApp.Models.Commons;

namespace FlashCardApp.Models.Entities;

public class User<TKey, TFlashCardSetKey, TFlashCardKey, TStudySessionKey, TCardReviewKey> : AuditableEntity<TKey>
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;

    // Navigation properties
    public ICollection<FlashCardSet<TFlashCardSetKey, TFlashCardKey, TKey, TStudySessionKey, TCardReviewKey>> FlashCardSets { get; set; } = default!;
    public ICollection<StudySession<TStudySessionKey, TKey, TFlashCardSetKey, TFlashCardKey, TCardReviewKey>> StudySessions { get; set; } = default!;
}
