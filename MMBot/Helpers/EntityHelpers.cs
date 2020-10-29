using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Discord;
using MMBot.Data.Enums;

namespace MMBot.Helpers
{
    public static class EntityHelpers
    {
        private static BindingFlags cisBF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |  BindingFlags.Static | BindingFlags.IgnoreCase;

        public static string ChangeProperty<T>(this T m, string propertyName, string newValue) 
            where T : class
        {
            var message = string.Empty;
            var on = typeof(T).ToString().Split('.').LastOrDefault();

            try
            {
                var pr = m.GetType().GetProperty(propertyName, cisBF);
                if (pr != null)
                {
                    var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                    var safeValue = (newValue == null) ? null : Enum.TryParse(newValue.ToString(), out Role eNum) ? eNum : Convert.ChangeType(newValue, t);
                    var val = Convert.ChangeType(safeValue, t);

                    var oldPropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);
                    m.GetType().GetProperty(propertyName, cisBF)?.SetValue(m, val);
                    var newPropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);

                    message = $"The {on} {m} now uses {newPropValue} instead of {oldPropValue} as {propertyName}.";
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
                foreach(var p in m.GetType().GetProperties())
                    ChangeProperty(m, p.Name, updateWith.GetProperty(p.Name));
            }
            catch
            {
                // ignore
            }
        }

        public static string GetProperty<T>(this T m, string propertyName) 
            where T : class
        {
            var message = string.Empty;
            var on = typeof(T).ToString().Split('.').LastOrDefault();

            try
            {
                var pr = m.GetType().GetProperty(propertyName, cisBF);
                if (pr != null)
                {
                    var t = Nullable.GetUnderlyingType(pr.PropertyType) ?? pr.PropertyType;
                    var PropValue = m.GetType().GetProperty(propertyName, cisBF)?.GetValue(m, null);

                    message = $"The {on} {m} uses {PropValue} as {propertyName}.";
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
                    var val = Convert.ChangeType(pVal, t);

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

            return null;
        }

        public static string GetTablePropertiesWithValues<T>(this IEnumerable<T> mm)
            where T : class
        {
            var message = "```";

            try
            {
                var m = mm.FirstOrDefault();

                message += typeof(T).ToString().Split('.').LastOrDefault();
                message += Environment.NewLine;

                var pr = m.GetType().GetProperties(cisBF).Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute))).ToArray();

                var ss = pr.Select(p => new
                {
                    pVal = m.GetType().GetProperty(p.Name, cisBF)?.GetValue(m, null),
                    t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType,
                    p
                })
                .Select(p => new
                {
                    val = Convert.ChangeType(p.pVal, p.t),
                    header = p.p?.Name?.ToSentence(),
                    p = p.p 
                })
                .Select((p, i) => new
                {
                    i,
                    Value = (p.val?.ToString()) ?? "not set",
                    p.header
                })
                .GroupBy(p => p.i, (x, y) => new { Index = x, Values = y })
                .Select(x =>
                {
                    var result = string.Empty;
                    if(x.Index == 0)
                    {
                        result += string.Join(", ", x.Values.SelectMany(y => y.header));
                        result += Environment.NewLine;
                    }
                        
                    result += string.Join(", ", x.Values.SelectMany(y => y.Value));

                    return result;
                })
                .ToArray();

                message += string.Join(Environment.NewLine, ss);
                message += "```";

                return message; 
            }
            catch
            {
                // ignored
            }

            return null;
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
                    var safeValue = (value == null) ? null : Enum.TryParse(value.ToString(), out Role eNum) ? eNum : Convert.ChangeType(value, t);
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

            return null;
        }
    }
}
