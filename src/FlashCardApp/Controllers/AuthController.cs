using FlashCardApp.Utils;
using Microsoft.AspNetCore.Mvc;
using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.Auth)]
[Tags(AppConstants.Tags.Auth)]
public class AuthController : ControllerBase
{
    
    [HttpPost("login_by_username",Name = "login_by_username")]
    [EndpointSummary("Login by username")]
    [EndpointDescription("Login by username with username and password")]
    [ProducesResponseType(typeof(DataResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult LoginByUsername([FromBody] LoginByUsernameRequest request)
    {
        // Implement login logic here
        return Ok();
    }

    [HttpPost("login_by_email",Name = "login_by_email")]
    [EndpointSummary("Login by email")]
    [EndpointDescription("Login by email with email and password")]
    [ProducesResponseType(typeof(DataResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult LoginByEmail([FromBody] LoginByEmailRequest request)
    {
        // Implement login by email logic here
        return Ok();
    }

    [HttpPost("logout",Name = "logout")]
    [EndpointSummary("Logout")]
    [EndpointDescription("Logout the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize]
    public IActionResult Logout()
    {
        // Implement logout logic here
        return Ok();
    }

    [HttpPost("register",Name = "register")]
    [EndpointSummary("Register a new user")]
    [EndpointDescription("Register a new user with username, email, and password")]
    [ProducesResponseType(typeof(DataResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Implement registration logic here
        return Ok();
    }

    [HttpPost("token_refresh", Name = "token_refresh")]
    [EndpointSummary("Refresh authentication token")]
    [EndpointDescription("Refresh the authentication token using a valid refresh token")]
    [ProducesResponseType(typeof(DataResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Implement refresh token logic here
        return Ok();
    }
}