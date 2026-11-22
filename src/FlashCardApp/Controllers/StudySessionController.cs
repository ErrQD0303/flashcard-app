using FlashCardApp.Dtos;
using FlashCardApp.Enums;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.StudySessions)]
[Tags(AppConstants.Tags.StudySession)]
[Authorize]
public class StudySessionController : ControllerBase
{
    [HttpPost("start/{flashCardSetId}", Name = "StartStudySession")]
    [EndpointSummary("Start a study session for the specified flash card set.")]
    [EndpointDescription("Starts a study session for the specified flash card set.")]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    public IActionResult StartStudySession(string flashCardSetId)
    {
        var createdStudySession = new CreatedStudySessionResponse
        {
            Id = "SampleId",
            FlashCardSetId = flashCardSetId,
            UserId = "SampleUserId",
            Mode = StudyMode.Flashcards,
            StartedAt = DateTime.UtcNow,
            IsActive = true
        };

        var response = DataResponse<CreatedStudySessionResponse>.Ok(createdStudySession, "Study session created successfully.");

        return CreatedAtAction(
            actionName: nameof(GetStudySessionById),
            routeValues: new { id = createdStudySession.Id, flashCardSetId = createdStudySession.FlashCardSetId },
            value: response
        );
    }

    [HttpPost("end/{flashCardSetId}", Name = "EndStudySession")]
    [EndpointSummary("End the current study session.")]
    [EndpointDescription("Ends the current study session for the user.")]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    public IActionResult EndStudySession(string flashCardSetId, [FromBody] StudySessionEndRequestDto request)
    {
        return Ok();
    }

    [HttpGet("", Name = "GetStudySessions")]
    [EndpointSummary("Get all study sessions for the specified flash card set.")]
    [EndpointDescription("Retrieves all study sessions for the specified flash card set.")]
    [ProducesResponseType(typeof(DataResponse<StudySessionResponseDto[]>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    public IActionResult GetStudySessions(string flashCardSetId)
    {
        var endedAt = DateTime.UtcNow;
        return Ok(
            DataResponse<StudySessionResponseDto[]>.Ok(
                [
                    new StudySessionResponseDto() {
                        Id = "SampleId",
                        FlashCardSetId = flashCardSetId,
                        UserId = "SampleUserId",
                        Mode = StudyMode.Flashcards,
                        StartedAt = DateTime.UtcNow,
                        EndedAt = endedAt,
                        IsActive = DateTime.UtcNow < endedAt
                    }
                ]
            )
        );
    }

    [HttpGet("{id}", Name = "GetStudySessionById")]
    [EndpointSummary("Get a study session by its ID.")]
    [EndpointDescription("Retrieves the study session with the specified ID.")]
    [ProducesResponseType(typeof(DataResponse<StudySessionResponseDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    public IActionResult GetStudySessionById(string id)
    {
        var endedAt = DateTime.UtcNow;
        return Ok(
            DataResponse<StudySessionResponseDto>.Ok(
                new StudySessionResponseDto()
                {
                    Id = id,
                    FlashCardSetId = string.Empty,
                    UserId = string.Empty,
                    Mode = StudyMode.Flashcards,
                    StartedAt = DateTime.UtcNow,
                    EndedAt = endedAt,
                    IsActive = DateTime.UtcNow < endedAt
                }
            )
        );
    }

    [HttpGet("current/{flashCardSetId}", Name = "GetCurrentActiveStudySession")]
    [EndpointSummary("Get the current active study session for the specified flash card set.")]
    [EndpointDescription("Retrieves the current active study session for the specified flash card set.")]
    [ProducesResponseType(typeof(DataResponse<StudySessionResponseDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>),StatusCodes.Status500InternalServerError)]
    public IActionResult GetCurrentActiveStudySession(string flashCardSetId)
    {
        var endedAt = DateTime.UtcNow;
        return Ok(
            DataResponse<StudySessionResponseDto>.Ok(
                new StudySessionResponseDto()
                {
                    Id = "SampleId",
                    FlashCardSetId = flashCardSetId,
                    UserId = "SampleUserId",
                    Mode = StudyMode.Flashcards,
                    StartedAt = DateTime.UtcNow,
                    EndedAt = endedAt,
                    IsActive = true // Since this is the current active study session, it is always active
                }
            )
        );
    }
}