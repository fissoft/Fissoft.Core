using System;
using System.Collections.Generic;
using Fissoft.EntitySearch;

namespace Fissoft.Transforms
{
    public class InTransformProvider : ITransformProvider
    {
        public bool Match(SearchItem item, Type type)
        {
            return item.Method == SearchMethod.In;
        }

        public IEnumerable<SearchItem> Transform(SearchItem item, Type type)
        {
            var arr = item.Value as Array;
            if (arr == null)
            {
                var arrStr = item.Value.ToString();
                if (!string.IsNullOrEmpty(arrStr))
                    arr = arrStr.Split(',');
            }
            return new[] {new SearchItem(item.Field, SearchMethod.StdIn, arr)};
        }
    }
}