using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fissoft.LinqIndex.Internal;

namespace Fissoft.LinqIndex.Indexes
{
    internal class DictionaryHashIndex<T> : IIndex<T>
    {
        protected readonly PropertyReader<T> PropertyReader;

        protected Dictionary<int, List<T>> IndexItems = new Dictionary<int, List<T>>();
        protected Dictionary<T, int> WhichIndexIsTIn = new Dictionary<T, int>();

        public DictionaryHashIndex(string propertyName, IEnumerable<T> items)
            : this(new IndexPropertySpecification(propertyName), items)
        {
        }

        public DictionaryHashIndex(IndexPropertySpecification indexPropertySpecification)
            : this(indexPropertySpecification, new List<T>())
        {
        }

        public DictionaryHashIndex(IndexPropertySpecification indexPropertySpecification, IEnumerable<T> items)
        {
            if (indexPropertySpecification == null)
                throw new ArgumentNullException("indexPropertySpecification");

            if (items == null)
                throw new ArgumentNullException("items");

            PropertyReader = new PropertyReader<T>(indexPropertySpecification);

            foreach (var item in items)
                AddToIndexInternal(item);
        }

        public IEnumerable<int> Keys => IndexItems.Keys;

        public bool TryGetItemsForKey(int key, out List<T> list)
        {
            return IndexItems.TryGetValue(key, out list);
        }

        public bool ContainsKey(int value)
        {
            return IndexItems.ContainsKey(value);
        }

        public IList<T> ItemsWithKey(int key)
        {
            return IndexItems[key];
        }

        public IEnumerator<KeyValuePair<int, IList<T>>> GetEnumerator()
        {
            var x = IndexItems.Select(s => new KeyValuePair<int, IList<T>>(s.Key, s.Value));
            return x.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void AddToIndex(T item)
        {
            AddToIndexInternal(item);
        }

        private void AddToIndexInternal(T item)
        {
            var hashCode = PropertyReader.GetItemHashCode(item);

            List<T> newItemsIndex;

            if (IndexItems.TryGetValue(hashCode, out newItemsIndex))
                newItemsIndex.Add(item);
            else
                IndexItems.Add(hashCode, new List<T> {item});

            if (!WhichIndexIsTIn.ContainsKey(item))
                WhichIndexIsTIn.Add(item, hashCode);
        }

        public virtual void RemoveFromIndex(T item)
        {
            RemoveFromIndexInternal(item);
        }

        private void RemoveFromIndexInternal(T item)
        {
            int foundHashCode;
            if (WhichIndexIsTIn.TryGetValue(item, out foundHashCode))
            {
                var convertedIndexFound = IndexItems[foundHashCode];

                convertedIndexFound.Remove(item);

                if (convertedIndexFound.Count != 0)
                    return;

                IndexItems.Remove(foundHashCode);
            }
        }
    }
}