using System.Collections.Generic;

namespace Fissoft.Framework.Systems.Data.Indexes
{
    public interface IIndex<T> : IEnumerable<KeyValuePair<int, IList<T>>>
    {
        IEnumerable<int> Keys { get; }
        bool TryGetItemsForKey(int key, out List<T> list);
        IList<T> ItemsWithKey(int key);
        bool ContainsKey(int key);
    }
}