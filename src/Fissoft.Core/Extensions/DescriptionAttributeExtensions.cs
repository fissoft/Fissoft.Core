using System;
using System.ComponentModel;
using System.Reflection;
using Fissoft.Framework.Systems.Attributes;
using System.Linq;

namespace Fissoft.Framework.Systems
{
    public static class DescriptionAttributeExtensions
    {
        public static string GetDescription(this Enum e)
        {
            Type type = e.GetType();
            MemberInfo[] memInfo = type.GetTypeInfo().GetMember(e.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Count() > 0)
                {
                    return ((DescriptionAttribute)attrs.FirstOrDefault()).Description;
                }
            }
            return e.ToString();
        }

  
        
    }
}
