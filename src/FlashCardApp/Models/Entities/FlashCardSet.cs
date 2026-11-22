using System.ComponentModel.DataAnnotations.Schema;
using FlashCardApp.Enums;
using FlashCardApp.Models.Commons;

namespace FlashCardApp.Models.Entities;

public class FlashCardSet<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey> : AuditableEntity<TKey>
{
    public TOwnerKey OwnerId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public FlashCardSetVisibility Visibility { get; set; } = FlashCardSetVisibility.Private;
    public TKey SourceSetId { get; set; } = default!;

    // Navigation property for the self-referencing relationship with SourceSetId
    public FlashCardSet<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey>? SourceSet { get; set; } // The parent source set
    public ICollection<FlashCardSet<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey>>? DerivedSets { get; set; } // The children derived from the parent source set
    public ICollection<FlashCard<TKey, TFlashCardKey, TOwnerKey, TStudySessionKey, TCardReviewKey>>? FlashCards { get; set; } // The flashcards belonging to this set
    public User<TOwnerKey, TKey, TFlashCardKey, TStudySessionKey, TCardReviewKey> User { get; set; } = default!;
    public ICollection<StudySession<TStudySessionKey, TOwnerKey, TKey, TFlashCardKey, TCardReviewKey>>? StudySessions { get; set; } // The study sessions associated with this flashcard set
}
