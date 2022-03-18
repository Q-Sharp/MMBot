using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using Discord;
using MMBot.Data.Contracts;
using MMBot.Data.Contracts.Enums;

namespace MMBot.Discord.Helpers;

public static class PropertyHelpers
{
    private static readonly BindingFlags cisBF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;

    public static string ChangeProperty<T>(this T m, string propertyName, string newValue, bool returnMessage = true)
        where T : class
    {
        var message = string.Empty;
        var on = typeof(T).ToString().Split('.').LastOrDefault();

        try
        {
            var pr = m.GetType().GetProperty(propertyName, cisBF);
            if (pr is not null)
            {
                var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                object safeValue = null;

                if (t is not null)
                    if (Enum.TryParse(newValue.ToString(), out Role eNum))
                        safeValue = eNum;
                    else if (TimeSpan.TryParse(newValue.ToString(), out var tSpan))
                        safeValue = tSpan;
                    else
                        safeValue = Convert.ChangeType(newValue, t);

                var val = Convert.ChangeType(safeValue, t);
                var oldPropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);
                m.GetType().GetProperty(propertyName, cisBF)?.SetValue(m, val);
                var newPropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);

                message = returnMessage ? $"The {on} {m} now uses {newPropValue} instead of {oldPropValue} as {propertyName}." : newPropValue.ToString();
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }

        return message;
    }

    public static void ChangeProperties<T>(this T m, T updateWith)
        where T : class
    {
        try
        {
            foreach (var p in m.GetType().GetProperties().Where(x => x.Name != nameof(IHaveId.Id)
                 && !x.CustomAttributes.Any(x => x.AttributeType == typeof(JsonIgnoreAttribute))))
                m.ChangeProperty(p.Name, updateWith.GetProperty(p.Name, false), false);
        }
        catch
        {
            // ignore
        }
    }

    public static string GetProperty<T>(this T m, string propertyName, bool returnMessage = true)
        where T : class
    {
        var message = string.Empty;
        var on = typeof(T).ToString().Split('.').LastOrDefault();

        try
        {
            var pr = m.GetType().GetProperty(propertyName, cisBF);
            if (pr is not null)
            {
                var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                var PropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);

                message = returnMessage ? $"The {on} {m} uses {PropValue} as {propertyName}." : PropValue.ToString();
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }

        return message;
    }

    public static IEmbed GetEmbedPropertiesWithValues<T>(this T m, string imageUrl = null)
        where T : class
    {
        var message = string.Empty;

        try
        {
            var builder = new EmbedBuilder
            {
                Color = Color.Purple,
                Title = typeof(T).ToString().Split('.').LastOrDefault(),
                ThumbnailUrl = imageUrl,
            };

            var pr = m.GetType().GetProperties(cisBF).Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute))).ToArray();

            foreach (var p in pr)
            {
                var pVal = m.GetType().GetProperty(p.Name, cisBF)?.GetValue(m, null);
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var val = pVal is null ? null : Convert.ChangeType(pVal, t);

                builder.AddField(x =>
                {
                    x.Name = p?.Name?.ToSentence();
                    x.Value = (val?.ToString()) ?? "not set";
                    x.IsInline = false;
                });
            }

            return builder.WithTimestamp(DateTimeOffset.Now).Build();
        }
        catch
        {
            // ignored
        }

        return default;
    }

    public static string GetTablePropertiesWithValues<T>(this IEnumerable<T> mm)
        where T : class
    {
        var message = "```";
        try
        {
            message += typeof(T).ToString().Split('.').LastOrDefault();
            message += Environment.NewLine;

            var ss = mm.Select(m => new
            {
                pr = m.GetType().GetProperties(cisBF).Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute))).ToArray(),
                m
            })
            .Select(p => new
            {
                pName = p?.pr?.Select(x => x.Name).ToArray(),
                p.m
            })
            .Select(p => new
            {
                values = p.pName.Select(x => p.m.GetProperty(x, false)).ToArray(),
                header = p.pName.Select(x => x?.ToSentence() ?? "").ToArray(),
            })
            .Select(x => new { header = string.Join(", ", x.header).TrimEnd(new char[] { ',', ' ' }), values = string.Join(", ", x.values).TrimEnd(new char[] { ',', ' ' }) })
            .ToArray();

            message += ss.FirstOrDefault().header;
            message += Environment.NewLine;
            message += string.Join(Environment.NewLine, ss.Select(s => s.values));
            message = message.TrimEnd(Environment.NewLine.ToCharArray());

            message += "```";
            return message;
        }
        catch
        {
            // ignored
        }

        return default;
    }

    public static IEnumerable<T> FilterCollectionByPropertyWithValue<T>(this IEnumerable<T> ml, string propertyName, string value)
        where T : class
    {
        try
        {
            return ml.Where(m =>
            {
                var pr = m.GetType().GetProperty(propertyName, cisBF);

                var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                var safeValue = value is null ? null : Enum.TryParse(value.ToString(), out Role eNum) ? eNum : Convert.ChangeType(value, t);
                var val = Convert.ChangeType(safeValue, t);

                var actPropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);

                return Comparer.DefaultInvariant.Compare(actPropValue, val) == 0;
            })
            .ToList();
        }
        catch
        {
            // ignored
        }

        return default;
    }

    public static object GetPropertyValue(this object src, string propName)
    {
        try
        {
            if (src is null || propName is null)
                return default;

            if (propName.Contains("."))
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
