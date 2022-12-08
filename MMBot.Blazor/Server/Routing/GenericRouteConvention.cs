namespace MMBot.Blazor.Server.Routing;

public class GenericRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            var genericType = controller.ControllerType.GenericTypeArguments.FirstOrDefault();
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"api/{genericType.Name}")),
            });
        }
    }
}
