using System.Reflection;

namespace Fissoft.Internal
{
    internal class ReflectionStringMethodCache
    {
        internal static MethodInfo Contains = typeof(string).GetTypeInfo().GetMethod("Contains", new[] { typeof(string) });

        internal static MethodInfo StartsWith =
            typeof(string).GetTypeInfo().GetMethod("StartsWith", new[] {typeof(string)});

        internal static MethodInfo EndsWith =
            typeof(string).GetTypeInfo().GetMethod("EndsWith", new[] {typeof(string)});
    }
}