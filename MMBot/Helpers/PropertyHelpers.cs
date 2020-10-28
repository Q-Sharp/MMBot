namespace MMBot.Helpers
{
    public static class PropertyHelpers
    {
        public static object GetPropertyValue(this object src, string propName)
        {
            try
            {
                if (src is null || propName is null)
                    return null;

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
    }
}
