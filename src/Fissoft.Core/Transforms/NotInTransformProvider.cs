using System;
using System.Collections.Generic;
using Fissoft.EntitySearch;
using System.Linq;

namespace Fissoft.Transforms
{
    public class NotInTransformProvider : ITransformProvider
    {
        public bool Match(SearchItem item, Type type)
        {
            return item.Method == SearchMethod.NotIn;
        }

        public IEnumerable<SearchItem> Transform(SearchItem item, Type type)
        {
            var arr = item.Value as Array;
            if (arr == null)
            {
                if (!(item.Value is IList<int> list))
                {
                    var arrStr = item.Value.ToString();
                    if (!string.IsNullOrEmpty(arrStr))
                        arr = arrStr.Split(',');
                }
                else
                {
                    arr = list.ToArray();
                }
            }
            return new[] {new SearchItem(item.Field, SearchMethod.StdNotIn, arr)};
        }
    }
}