using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Discord;
using Microsoft.EntityFrameworkCore.Internal;

namespace TTMMBot.Data.Entities
{
    public static class EntityHelpers
    {
        public static string ChangeProperty<T>(this T m, string propertyName, string newValue) 
            where T : class
        {
            var message = string.Empty;
            var on = typeof(T).ToString().Split('.').LastOrDefault();

            try
            {
                var pr = m.GetType().GetRuntimeProperty(propertyName);
                if (pr != null)
                {
                    var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                    var safeValue = (newValue == null) ? null : Convert.ChangeType(newValue, t);
                    var val = Convert.ChangeType(safeValue, t);

                    var oldPropValue = m.GetType().GetProperty(propertyName)?.GetValue(m, null);
                    m.GetType().GetProperty(propertyName)?.SetValue(m, val);
                    var newPropValue = m.GetType().GetProperty(propertyName)?.GetValue(m, null);

                    message = $"The {on} {m} now uses {newPropValue} instead of {oldPropValue} as {propertyName}.";
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
            var on = typeof(T).ToString().Split('.').LastOrDefault();

            try
            {
                var builder = new EmbedBuilder
                {
                    Color = Color.Purple,
                    Title = typeof(T).ToString().Split('.').LastOrDefault(),
                    ThumbnailUrl = imageUrl,
                };

                var pr = m.GetType().GetRuntimeProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute))).ToArray();

                foreach (var p in pr)
                {
                    var pVal = m.GetType().GetProperty(p.Name)?.GetValue(m, null);
                    var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var val = Convert.ChangeType(pVal, t);

                    builder.AddField(x =>
                    {
                        x.Name = p.Name;
                        x.Value = val?.ToString();
                        x.IsInline = false;
                    });
                }

                return builder.WithTimestamp(DateTimeOffset.Now).Build();
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}
