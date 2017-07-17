using System;
using System.Collections.Generic;
using Fissoft.EntitySearch;

namespace Fissoft.Transforms
{
    public class DateBlockTransformProvider : ITransformProvider
    {
        #region ITransformProvider Members

        public bool Match(SearchItem item, Type type)
        {
            return item.Method == SearchMethod.DateBlock;
        }

        public IEnumerable<SearchItem> Transform(SearchItem item, Type type)
        {
            return new[]
                       {
                           new SearchItem(item.Field, SearchMethod.GreaterThanOrEqual, item.Value),
                           new SearchItem(item.Field, SearchMethod.LessThan, item.Value)
                       };
        }

        #endregion
    }
}