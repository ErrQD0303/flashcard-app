using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.FlashCardSets + "/{flashCardSetId}/" + AppConstants.ApiRoutes.FlashCards)]
[Tags(AppConstants.Tags.FlashCards)]
[Authorize]
public class FlashCardController : ControllerBase
{
    [HttpGet(Name = "get_flash_cards")]
    [EndpointSummary("Get all flash cards within a flash card set")]
    [EndpointDescription("Retrieve all flash cards within a specific flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<FlashCardResponseDto[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetFlashCards(string flashCardSetId)
    {
        // Placeholder for retrieving all flash cards within a flash card set
        return Ok(new DataResponse<FlashCardResponseDto[]>
        {
            Data = []
        });
    }

    [HttpGet("{id}", Name = "get_flash_card")]
    [EndpointSummary("Get a specific flash card within a flash card set")]
    [EndpointDescription("Retrieve a specific flash card within a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<FlashCardResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetFlashCard(string flashCardSetId, string id)
    {
        // Placeholder for retrieving a specific flash card within a flash card set
        return Ok(new DataResponse<FlashCardResponseDto>
        {
            Data = new FlashCardResponseDto()
        });
    }

    [HttpPost(Name = "create_flash_card")]
    [EndpointSummary("Create a new flash card within a flash card set")]
    [EndpointDescription("Create a new flash card within a specific flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<CreatedFlashCardResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult CreateFlashCard(string flashCardSetId,[FromBody] CreateFlashCardRequest request)
    {
        // Placeholder for creating a new flash card within a flash card set
        return CreatedAtRoute("get_flash_card", new { id = "new-id" }, new DataResponse<CreatedFlashCardResponse>
        {
            Data = new CreatedFlashCardResponse()
        });
    }

    [HttpDelete("{id}", Name = "delete_flash_card")]
    [EndpointSummary("Delete a flash card within a set")]
    [EndpointDescription("Delete a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteFlashCard(string id)
    {
        // Placeholder for deleting a flash card within a set
        return NoContent();
    }

    [HttpPatch("{id}", Name = "update_flash_card")]
    [EndpointSummary("Update a flash card within a set")]
    [EndpointDescription("Update a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateFlashCard(string flashCardSetId, string id, [FromBody] UpdateFlashCardRequest updateFlashCard)
    {
        // Placeholder for updating a flash card within a set
        return NoContent();
    }

    [HttpPatch("{id}/position", Name = "update_flash_card_position")]
    [EndpointSummary("Update the position of a flash card within a set")]
    [EndpointDescription("Update the position of a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateFlashCardPosition(string flashCardSetId, string id, [FromBody] UpdateFlashCardPositionRequest updateFlashCardPosition)
    {
        // Placeholder for updating the position of a flash card within a set
        return NoContent();
    }
}
