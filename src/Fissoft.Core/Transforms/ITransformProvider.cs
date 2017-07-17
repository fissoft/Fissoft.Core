using System;
using System.Collections.Generic;
using Fissoft.EntitySearch;

namespace Fissoft.Transforms
{
    public interface ITransformProvider
    {
        bool Match(SearchItem item, Type type);
        IEnumerable<SearchItem> Transform(SearchItem item, Type type);
    }
}