using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fissoft
{
    public static class DisplayNameAttributeExtensions
    {
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false).ToArray();
            if (attrs.Any())
                return ((DisplayNameAttribute) attrs.FirstOrDefault())?.DisplayName;
            return propertyInfo.Name;
        }
    }
}