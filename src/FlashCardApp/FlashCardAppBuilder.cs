using System.Text.Json;
using System.Text.Json.Serialization;
using FlashCardApp.Data.Contracts;
using FlashCardApp.Dtos;
using FlashCardApp.ExceptionHandlers;
using FlashCardApp.ModelConventions;
using FlashCardApp.Options;
using FlashCardApp.Swashbuckle.Filters;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace FlashCardApp;

public static class FlashCardAppBuilder
{
    public static WebApplicationBuilder AddFlashCardApp(this WebApplicationBuilder builder)
    {
        // Config the Repository and associated services for the FlashCardApp
        // builder.Services.AddTodoApi();

        // Options
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(AppConstants.App_Options.JwtOptionsSection));
        builder.Services.AddOptions<JwtOptions>()
            .ValidateDataAnnotations() // Validate the options using data annotations attributes like [Required] and [MinLength]
            .ValidateOnStart(); // Validate the options when the application starts to catch misconfigurations early

        // Add Authentication using JWT Bearer scheme
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                static void EnsureJwtOptionsConfigsValid(JwtOptions? jwtOptions)
                {
                    if (jwtOptions == null)
                    {
                        throw new InvalidOperationException("JWT options are not configured.");
                    }

                    if (string.IsNullOrEmpty(jwtOptions.Authority))
                    {
                        throw new InvalidOperationException("JWT Authority is not configured.");
                    }
                    if (string.IsNullOrEmpty(jwtOptions.Audience))
                    {
                        throw new InvalidOperationException("JWT Audience is not configured.");
                    }
                    if (string.IsNullOrEmpty(jwtOptions.SecretKey))
                    {
                        throw new InvalidOperationException("JWT SecretKey is not configured.");
                    }
                }
                // Read the JWT options from configuration
                // Ensure that the JWT options are properly configured
                var jwtOptions = builder.Configuration.GetSection(AppConstants.App_Options.JwtOptionsSection).Get<JwtOptions>();

                EnsureJwtOptionsConfigsValid(jwtOptions);

                // Configure JWT Bearer options here if needed
                // options.Authority = jwtOptions!.Authority;
                options.Audience = jwtOptions!.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtOptions!.Audience,
                    ValidIssuer = jwtOptions!.Authority,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions!.SecretKey!))
                };
            });

        // Add support for controllers with route prefix convention
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute(AppConstants.Default_Api_Prefix.Value)));

            // Use [JsonPropertyName]'s values for JSON serialization. This ensures that the model binding and validation errors use the JSON property names rather than the C# property names.
            options.ModelMetadataDetailsProviders.Add(
               new SystemTextJsonValidationMetadataProvider(JsonNamingPolicy.SnakeCaseLower
            )); // if the property has a [JsonPropertyName] attribute, its value will be used for model binding and validation errors. if not, the default naming policy (SnakeCaseLower) will be applied.
        }).ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressMapClientErrors = true; // Suppress automatic 4xx responses to allow custom handling in the UseStatusCodePages middleware
            // 422 Handling for invalid model state
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var isMalformedBody = errors.Keys.Any(k => k == string.Empty || k.StartsWith('$'));

                if (isMalformedBody)
                {
                    var bodyParameterNames = context.ActionDescriptor.Parameters
                        .Where(p => p.BindingInfo?.BindingSource == BindingSource.Body)
                        .Select(p => p.Name)
                        .ToHashSet();

                    var bodyErrors = errors
                        .Where(e => !bodyParameterNames.Contains(e.Key))
                        .ToDictionary(
                            e => e.Key.Length == 0 ? "body" : e.Key,
                            e => e.Value
                        );

                    return new BadRequestObjectResult(DataResponse<object>.Fail("Malformed request body.", bodyErrors));
                }

                var response = DataResponse<object>.Fail("Validation failed.", errors);
                return new UnprocessableEntityObjectResult(response);
            };
        })

        ; // Add support for controllers
        // builder.Services.AddEndpointsApiExplorer(); // Add support for API endpoint exploration for minimal APIs (Omit here since using controllers only)
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FlashCardApp API",
                Description = "An API for simulating Quizlet functionality.",
                Contact = new OpenApiContact
                {
                    Name = "ErrQD0303 (Dat Nguyen)",
                    Url = new Uri("https://support@flashcardapp.com")
                },
                Version = "v1"
            });

            options.OperationFilter<AddFlashCardAppTagFilter>();
            options.DocumentFilter<AddFlashCardAppGlobalTagFilter>();
        });

        // // Configure JSON serialization options for HTTP responses
        // builder.Services.ConfigureHttpJsonOptions(options =>
        // {
        //     options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        // });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<AppCoreDbContext<Guid, Guid, Guid, Guid, Guid>>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // Add any custom configuration for the FlashCardApp here
        return builder;
    }

    public static WebApplication UseFlashCardApp(this WebApplication app)
    {
        // 500 Internal Server Error handling
        app.UseExceptionHandler(); // Use the global exception handler for unhandled exceptions

        // Other 4xx error handling except 400, 401, 403 because UseStatusCodePages only fills empty responses that have not been written to yet
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            if (response.HasStarted)
            {
                return;
            }

            response.ContentType = "application/json";
            var message = response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status403Forbidden => "Forbidden",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status405MethodNotAllowed => "Method Not Allowed",
                StatusCodes.Status409Conflict => "Conflict",
                StatusCodes.Status415UnsupportedMediaType => "Unsupported Media Type",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => $"Request failed with status code {response.StatusCode}."
            };

            var dataResponse = DataResponse<object>.Fail(message);
            await response.WriteAsJsonAsync(dataResponse);
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication(); // Use authentication middleware, if no authentication scheme is configured, this will have no effect, a.k.a all requests will be treated as anonymous
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}