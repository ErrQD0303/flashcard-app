using FlashCardApp.Dtos;
using FlashCardApp.Enums;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.CardReviews)]
[Tags(AppConstants.Tags.CardReviews)]
[Authorize]
public class CardReviewController : ControllerBase
{
    [HttpGet("{cardId}", Name = "GetCardReviewsByCardId")]
    [EndpointSummary("Get the review for the specified card.")]
    [EndpointDescription("Retrieves the review details for the specified card.")]
    [ProducesResponseType(typeof(DataResponse<CardReviewResponseDto[]>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))] 
    public IActionResult GetCardReviewByCardId(string cardId)
    {
        return Ok(
            DataResponse<CardReviewResponseDto[]>.Ok([
                new CardReviewResponseDto() {
                    Id = "TemplateId",
                    SessionId = "TemplateSessionId",
                    CardId = "TemplateCardId",
                    Result = ReviewResult.Skipped,
                    ResponseTimeMs = 0,
                    ReviewedAt = DateTime.UtcNow
                }
            ])
        );
    }

    [HttpGet("{cardId}/" + AppConstants.ApiRoutes.StudySessions + "/{studySessionId}", Name = "GetCardReviewsByCardIdAndStudySessionId")]
    [EndpointSummary("Get the review for the specified card and study session.")]
    [EndpointDescription("Retrieves the review details for the specified card and study session.")]
    [ProducesResponseType(typeof(DataResponse<CardReviewResponseDto[]>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))] 
    public IActionResult GetCardReviewsByCardIdAndStudySessionId(string cardId, string studySessionId)
    {
        return Ok(
            DataResponse<CardReviewResponseDto[]>.Ok([
                new CardReviewResponseDto() {
                    Id = "TemplateId",
                    SessionId = "TemplateSessionId",
                    CardId = "TemplateCardId",
                    Result = ReviewResult.Skipped,
                    ResponseTimeMs = 0,
                    ReviewedAt = DateTime.UtcNow
                }
            ])
        );
    }
}