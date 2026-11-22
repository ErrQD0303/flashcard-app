using FlashCardApp.Models.Commons;

namespace FlashCardApp.Models.Entities;

public class FlashCard<TKey, TFlashCardSetKey, TOwnerKey, TStudySessionKey, TCardReviewKey> 
: AuditableEntity<TKey>
{
    public TFlashCardSetKey SetId { get; set; } = default!;
    public string Term { get; set; } = default!;
    public string Definition { get; set; } = default!;
    public int Position { get; set; } = default!;

    // Navigation property to the FlashCardSet this card belongs to
    public FlashCardSet<TKey,TFlashCardSetKey, TOwnerKey, TStudySessionKey, TCardReviewKey> Set { get; set; } = default!;
    public ICollection<CardReview<TCardReviewKey, TStudySessionKey, TKey, TOwnerKey, TFlashCardSetKey>> CardReviews { get; set; } = default!;
}
