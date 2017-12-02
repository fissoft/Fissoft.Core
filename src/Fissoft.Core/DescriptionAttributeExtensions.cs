using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fissoft
{
    public static class DescriptionAttributeExtensions
    {
        public static string GetDescription(this Enum e)
        {
            var type = e.GetType().GetTypeInfo();
            var memInfo = type.GetMember(e.ToString());
            if (memInfo == null || memInfo.Length <= 0) return e.ToString();
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray();
            if (attrs.Any())
                return ((DescriptionAttribute) attrs.FirstOrDefault())?.Description;
            return e.ToString();
        }
    }
}