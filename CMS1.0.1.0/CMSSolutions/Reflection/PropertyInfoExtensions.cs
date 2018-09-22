using System.Linq;
using System.Reflection;

namespace CMSSolutions.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static T GetAttributeOfType<T>(this PropertyInfo propertyInfo) where T : class
        {
            return propertyInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
        }
    }
}