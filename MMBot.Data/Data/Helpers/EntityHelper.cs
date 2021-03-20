using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Helpers
{
    public static class EntityHelper
    {
        public async static Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, T updateWithData, ulong guildId)
            where T : class, IHaveId, IHaveIdentifier 
                => await ImportOrUpgradeWithIdentifier(currentData, new List<T> { updateWithData }, guildId);

        public async static Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, IList<T> updateWithData, ulong guildId) 
            where T : class, IHaveId, IHaveIdentifier
        {
            foreach(var uwd in updateWithData)
            {
                var found = currentData.AsQueryable().FirstOrDefault(x => x.Name == uwd.Name && x.Identitfier == uwd.Identitfier && uwd.GuildId == guildId);

                if(found is not null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }

        public async static Task ImportOrUpgrade<T>(this DbSet<T> currentData, IList<T> updateWithData) 
            where T : class, IHaveId
        {
            foreach(var uwd in updateWithData)
            {
                var found = await currentData.FindAsync(uwd.Id);

                if(found is not null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }

        public static IEnumerable<string> GetHeader<T>(this T e) => GetHeader<T>();
        public static IEnumerable<string> GetHeader<T>() => typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute))).Select(x => x.Name);
        public static IEnumerable<string> GetValues<T>(this T e)
        {
            var p = typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(DisplayAttribute) && x.AttributeType != typeof(IgnoreDataMemberAttribute)));
            return p.Select(x => e.GetPropertyValue(x.Name) as string);
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
                    return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
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

        public static TType TryCast<TType>(this object src)
            where TType : class
        {
            try
            {
                return src as TType;
            }
            catch
            {
                return null;
            }
        }
    }
}
