using System.Reflection;

namespace Fissoft.Framework.Systems.Common
{
    class ReflectionStringMethodCache
    {
        static internal MethodInfo Contains = typeof(string).GetMethod("Contains");
        internal static MethodInfo StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        internal static MethodInfo EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

    }
}
