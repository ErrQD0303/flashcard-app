namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
public class StudySessionEndRequestDto
{
    [Required]
    [Description("The list of card reviews for the study session.")]
    public StudySessionEndRequestCardReviewDto[] CardReviews { get; set; } = [];

    [Required]
    [Description("The timestamp when the study session was reviewed.")]
    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
}
