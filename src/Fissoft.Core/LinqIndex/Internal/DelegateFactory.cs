using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Fissoft.LinqIndex.Internal
{
    internal delegate object LateBoundProperty(object target);

    internal static class DelegateFactory
    {
        public static LateBoundProperty Create<T>(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var method = typeof(T).GetTypeInfo().GetMethod("get_" + property.Name, Type.EmptyTypes);

            return Create(method);
        }

        private static LateBoundProperty Create(MethodInfo method)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");

            var call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method);

            var lambda = Expression.Lambda<LateBoundProperty>(
                Expression.Convert(call, typeof(object)),
                instanceParameter);

            return lambda.Compile();
        }
    }
}