namespace MMBot.Blazor.Server.Routing;

public class GenericFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var currentAssembly = typeof(IHaveId).Assembly;
        var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetInterface(nameof(IHaveGuildId)) is not null);

        foreach (var candidate in candidates)
            feature.Controllers.Add(
                typeof(EntityController<>).MakeGenericType(candidate).GetTypeInfo()
            );
    }
}
