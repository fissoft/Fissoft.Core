using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fissoft.LinqIndex.Internal
{
    internal static class EnumerableExtensions
    {
        public static Collection<T> ToCollection<T>(this IEnumerable<T> items)
        {
            var c = new Collection<T>();

            if (items == null)
                return c;

            foreach (var item in items)
                c.Add(item);

            return c;
        }
    }
}