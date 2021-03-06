﻿using System.Collections.Generic;
using System.Collections.Specialized;
using Fissoft.LinqIndex.Indexes;

namespace Fissoft.LinqIndex.Internal
{
    internal static class InternalObservablesHook
    {
        internal static readonly Dictionary<object, object> ItemsMonitoring = new Dictionary<object, object>();

        public static void Add<TCollection, T>(TCollection collection, IndexSpecification<T> spec)
            where TCollection : class, INotifyCollectionChanged, IEnumerable<T>
        {
            var observableIndexableCollection = new ObservableIndexableCollection<TCollection, T>(collection, spec);

            ItemsMonitoring.Add(collection, observableIndexableCollection);
        }

        public static void Remove(object collectoin)
        {
            ItemsMonitoring.Remove(collectoin);
        }

        public static bool TryGetIndexForObservable<T>(object collection,
            out IIndexableCollection<T> indexableCollection)
        {
            if (ItemsMonitoring.TryGetValue(collection, out var o))
            {
                indexableCollection = (IIndexableCollection<T>) o;

                return true;
            }

            indexableCollection = null;
            return false;
        }


        internal static void ResetForTestabilityPurposes()
        {
            ItemsMonitoring.Clear();
        }

        public static bool ContainsCollection(object collection)
        {
            return ItemsMonitoring.ContainsKey(collection);
        }
    }
}