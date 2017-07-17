using System;
using System.ComponentModel;
using System.Reflection;

namespace Fissoft.Framework.Systems
{
    public static class DisplayNameAttributeExtensions
    {
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((DisplayNameAttribute)attrs[0]).DisplayName;            
            return propertyInfo.Name;
        }
        
    }
}
