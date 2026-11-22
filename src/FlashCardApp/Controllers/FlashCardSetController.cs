using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;
using FlashCardApp.Enums;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.FlashCardSets)]
[Tags(AppConstants.Tags.FlashCardSets)]
[Authorize]
public class FlashCardSetController : ControllerBase
{
    
    [HttpGet(Name = "get_flash_card_sets")] // Fetch all flash card sets by GET /api/flash-card-sets
    [EndpointSummary("Get all flash card sets")]
    [EndpointDescription("Get all of the current user's flash card sets")]
    [ProducesResponseType(typeof(DataResponse<FlashCardSetResponseDto[]>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult GetFlashCardSets()
    {
        // Placeholder for fetching flash card sets
        return Ok(DataResponse<FlashCardSetResponseDto[]>.Ok([
            new FlashCardSetResponseDto
            {
                Id = "SampleId",
                Title = "Sample FlashCardSet",
                Description = "Sample description for the flash card set",
                FlashCards = [],
                OwnerId = "SampleOwnerId",
                Visibility = FlashCardSetVisibility.Public
            }
        ]));
    }

    [HttpGet("{id}", Name = "get_flash_card_set_by_id")] // Fetch a flash card set by GET /api/flash-card-sets/{id}
    [EndpointSummary("Get a flash card set by ID")]
    [EndpointDescription("Get a specific flash card set for the current user by its ID")]
    [ProducesResponseType(typeof(DataResponse<FlashCardSetResponseDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetFlashCardSet(string id)
    {
        // Placeholder for fetching a flash card set by ID
        return Ok(DataResponse<FlashCardSetResponseDto>.Ok(new FlashCardSetResponseDto
        {
            Id = id,
            Title = "Sample FlashCardSet",
            Description = "Sample description for the flash card set",
            FlashCards = [],
            OwnerId = "SampleOwnerId",
            Visibility = FlashCardSetVisibility.Public
        }));
    }

    [HttpPost(Name = "create_flash_card_set")] // Create a new flash card set by POST /api/flash-card-sets
    [EndpointSummary("Create a new flash card set")]
    [EndpointDescription("Create a new flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<CreatedFlashCardSetResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult CreateFlashCardSet([FromBody] CreateFlashCardSetRequest flashCardSet)
    {  // 1. Save to database and generate the new ID
        var createdSet = new CreatedFlashCardSetResponse 
        { 
            Id = "SampleId", // Replace with actual generated ID
            Title = flashCardSet.Title,
            Description = flashCardSet.Description,
            FlashCards = [],
            OwnerId = "SampleOwnerId",
            Visibility = FlashCardSetVisibility.Public
        };

        var response = DataResponse<CreatedFlashCardSetResponse>.Ok(createdSet, "Flash card set created successfully");

        // 2. Pass routeValues (must match the parameter name in GetFlashCardSetById)
        return CreatedAtAction(
            actionName: nameof(GetFlashCardSet),
            routeValues: new { id = createdSet.Id },
            value: response
        ); 
    }

    [HttpDelete("{id}", Name = "delete_flash_card_set")] // Delete a flash card set by DELETE /api/flash-card-sets/{id}
    [EndpointSummary("Delete a flash card set")]
    [EndpointDescription("Delete a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult DeleteFlashCardSet(string id)
    {
        // Placeholder for deleting a flash card set
        return NoContent();
    }

    [HttpPut("{id}", Name = "update_flash_card_set")] // Update a flash card set by PUT /api/flash-card-sets/{id}
    [EndpointSummary("Update a flash card set")]
    [EndpointDescription("Update a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCardSet(string id, [FromBody] UpdateFlashCardSetRequest flashCardSet)
    {
        // Placeholder for updating a flash card set
        return NoContent();
    }

    [HttpPut("{id}/visibility", Name = "update_flash_card_set_visibility")] // Update the visibility of a flash card set
    [EndpointSummary("Update the visibility of a flash card set")]
    [EndpointDescription("Update the visibility of a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCardSetVisibility(string id, [FromBody] UpdateFlashCardSetVisibilityRequest updateFlashCardSetVisibility)
    {
        // Placeholder for updating the visibility of a flash card set
        return NoContent();
    }
}
