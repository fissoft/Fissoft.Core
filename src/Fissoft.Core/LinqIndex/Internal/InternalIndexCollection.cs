using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fissoft.LinqIndex.Indexes;

namespace Fissoft.LinqIndex.Internal
{
    internal class InternalIndexCollection<T> : IEnumerable<DictionaryHashIndex<T>>
    {
        private readonly Dictionary<string, DictionaryHashIndex<T>> _internalIndexList =
            new Dictionary<string, DictionaryHashIndex<T>>();

        public IEnumerator<DictionaryHashIndex<T>> GetEnumerator()
        {
            return _internalIndexList.Select(s => s.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsIndex(string propertyName)
        {
            return _internalIndexList.ContainsKey(propertyName);
        }

        public bool RemoveIndex(string propertyName)
        {
            if (_internalIndexList.ContainsKey(propertyName))
                return _internalIndexList.Remove(propertyName);
            return false;
        }

        public void AddIndexFor(IndexPropertySpecification indexPropertySpecification, DictionaryHashIndex<T> index)
        {
            _internalIndexList.Add(indexPropertySpecification.PropertyName, index);
        }

        public DictionaryHashIndex<T> GetIndexByPropertyName(string propertyName)
        {
            return _internalIndexList[propertyName];
        }

        public void Clear()
        {
            _internalIndexList.Clear();
        }
    }
}