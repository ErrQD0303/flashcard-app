namespace FlashCardApp.Dtos;
using FlashCardApp.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class StudySessionEndRequestCardReviewDto
{
    [Required]
    [Description("The unique identifier of the card being reviewed.")]
    public string CardId { get; set; } = string.Empty;
    
    [Required]
    [Description("The result of the card review.")]
    public ReviewResult Result { get; set; } = ReviewResult.Skipped;

    [Required]
    [Description("The response time for the card review in milliseconds.")]
    public int ResponseTimeMs { get; set; } = 0;
}