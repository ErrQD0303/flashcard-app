using FlashCardApp.Utils;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlashCardApp.Swashbuckle.Filters;

public class AddFlashCardAppGlobalTagFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags ??= new HashSet<OpenApiTag>();

        if (!swaggerDoc.Tags.Any(tag => tag.Name == AppConstants.Tags.FlashCardApp))
        {
            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = AppConstants.Tags.FlashCardApp,
                Description = "All FlashCardApp endpoints"
            });
        }
    }
}