using FlashCardApp.Utils;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlashCardApp.Swashbuckle.Filters;

public class AddFlashCardAppTagFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Tags ??= new HashSet<OpenApiTagReference>();

        if (!operation.Tags.Any(t => t.Name == AppConstants.Tags.FlashCardApp))
        {
            operation.Tags.Add(new OpenApiTagReference(AppConstants.Tags.FlashCardApp));
        }
    }
}