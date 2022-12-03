namespace MMBot.Data.Helpers;

public static class EntityHelper
{
    public static async Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, T updateWithData, ulong guildId)
        where T : class, IHaveId, IHaveIdentifier
            => await currentData.ImportOrUpgradeWithIdentifier(new List<T> { updateWithData }, guildId);

    public static async Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, IList<T> updateWithData, ulong guildId)
        where T : class, IHaveId, IHaveIdentifier
    {
        foreach (var uwd in updateWithData)
        {
            var found = currentData.AsQueryable().FirstOrDefault(x => x.Name == uwd.Name && x.Identitfier == uwd.Identitfier && uwd.GuildId == guildId);

            if (found is not null)
                found.Update(uwd);
            else
                 await currentData.AddAsync(uwd);
        }
    }

    public static async Task ImportOrUpgrade<T>(this DbSet<T> currentData, IList<T> updateWithData)
        where T : class, IHaveId
    {
        foreach (var uwd in updateWithData)
        {
            var found = await currentData.FindAsync(uwd.Id);

            if (found is not null)
                found.Update(uwd);
            else
                 await currentData.AddAsync(uwd);
        }
    }

    public static IEnumerable<string> GetHeader<T>(this T e) => e.GetHeader();
    public static IEnumerable<string> GetHeader<T>() => GetProperties<T>()
        .Select(x => x.Name);

    public static IEnumerable<string> GetValues<T>(this T e)
    {
        var p = GetProperties<T>();
        return p.Select(x => e.GetPropertyValue(x.Name) as string);
    }

    public static IEnumerable<PropertyInfo> GetProperties<T>()
        => typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(DisplayAttribute) && y.AttributeType != typeof(IgnoreDataMemberAttribute)));

    public static object GetPropertyValue(this object src, string propName)
    {
        try
        {
            if (src is null || propName is null)
                return default;

            if (propName.Contains('.'))
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return src.GetPropertyValue(temp[0]).GetPropertyValue(temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop?.GetValue(src, null);
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
