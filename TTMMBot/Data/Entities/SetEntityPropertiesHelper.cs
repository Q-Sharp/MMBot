using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TTMMBot.Data.Entities
{
    public static class SetEntityPropertiesHelper
    {
        public async static Task ChangeProperty<T>(T m, string propertyName, string newValue, Func<string, Task> outPut)
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
            finally
            {
                await outPut(message);
            }
        }
    }
}
