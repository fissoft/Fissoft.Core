using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Fissoft.LinqIndex.Internal
{
    internal static class ExtensionHelpers
    {
        public static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            return ((MemberExpression) propertyExpression.Body).Member.Name;
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }
    }
}