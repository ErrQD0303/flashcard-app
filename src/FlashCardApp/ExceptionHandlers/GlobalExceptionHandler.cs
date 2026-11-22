namespace FlashCardApp.ExceptionHandlers;

using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Diagnostics;

// Handles global exceptions and returns a standardized JSON response. (500 Internal Server Error)
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {exceptionMessage}", exception.Message);

        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        var message = _env.IsDevelopment() ? exception.Message : "Internal Server Error";

        var response = DataResponse<object>.Fail(message);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}