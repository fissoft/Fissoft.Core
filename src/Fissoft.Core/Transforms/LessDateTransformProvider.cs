using System;
using System.Collections.Generic;
using Fissoft.EntitySearch;

namespace Fissoft.Transforms
{
    public class LessDateTransformProvider : ITransformProvider
    {
        public bool Match(SearchItem item, Type type)
        {
            return true;
            
        }

        public IEnumerable<SearchItem> Transform(SearchItem item, Type type)
        {
            throw new NotImplementedException();
        }
    }
}