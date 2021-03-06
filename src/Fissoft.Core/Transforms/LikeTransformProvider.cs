using System;
using System.Collections.Generic;
using System.Linq;
using Fissoft.EntitySearch;

namespace Fissoft.Transforms
{
    public class LikeTransformProvider : ITransformProvider
    {
        public bool Match(SearchItem item, Type type)
        {
            return item.Method == SearchMethod.Like;
        }

        public IEnumerable<SearchItem> Transform(SearchItem item, Type type)
        {
            var str = item.Value.ToString();
            var keyWords = str.Split('*', '%');
            if (keyWords.Length == 1)
                return new[] {new SearchItem(item.Field, SearchMethod.Equal, item.Value)};
            var list = new List<SearchItem>();
            if (!string.IsNullOrEmpty(keyWords.First()))
                list.Add(new SearchItem(item.Field, SearchMethod.StartsWith, keyWords.First()));
            if (!string.IsNullOrEmpty(keyWords.Last()))
                list.Add(new SearchItem(item.Field, SearchMethod.EndsWith, keyWords.Last()));
            for (var i = 1; i < keyWords.Length - 1; i++)
                if (!string.IsNullOrEmpty(keyWords[i]))
                    list.Add(new SearchItem(item.Field, SearchMethod.Contains, keyWords[i]));
            return list;
        }
    }
}