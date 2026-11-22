using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FlashCardApp.ModelConventions;

public class RoutePrefixConvention : IApplicationModelConvention
{

    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(IRouteTemplateProvider route)
    {
        _routePrefix = new AttributeRouteModel(route);
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            if (selector.AttributeRouteModel != null)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                continue;
            }

            selector.AttributeRouteModel = _routePrefix;
        }
    }
}