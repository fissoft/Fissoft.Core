using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Fissoft.LinqIndex.Indexes;

namespace Fissoft.LinqIndex.Internal
{
    internal class ObservableIndexableCollection<TCollection, T> : IIndexableCollection<T>
        where TCollection : class, INotifyCollectionChanged, IEnumerable<T>
    {
        private readonly TCollection _items;
        private readonly IndexSpecification<T> _indexSpecification;

        private IndexableCollection<T> _internalIndexableCollection = new IndexableCollection<T>();

        public ObservableIndexableCollection(TCollection items)
            : this(items, new IndexSpecification<T>())
        {
        }

        public ObservableIndexableCollection(TCollection items, IndexSpecification<T> indexSpecification)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (indexSpecification == null)
                throw new ArgumentNullException("indexSpecification");

            _items = items;
            _indexSpecification = indexSpecification;

            _internalIndexableCollection = new IndexableCollection<T>(items, indexSpecification);

            items.CollectionChanged += OnItemsCollectionChanged;
        }

        static void SaveListActionWrapper(IList listToWorkWith, Action<T> actionToTake)
        {
            if (listToWorkWith != null)
                foreach (var item in listToWorkWith.Cast<T>())
                {
                    actionToTake(item);
                }
        }

        void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SaveListActionWrapper(e.NewItems, _internalIndexableCollection.Add);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                SaveListActionWrapper(e.OldItems, item => _internalIndexableCollection.Remove(item));
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                SaveListActionWrapper(e.OldItems, item => _internalIndexableCollection.Remove(item));
                SaveListActionWrapper(e.NewItems, _internalIndexableCollection.Add);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _internalIndexableCollection = new IndexableCollection<T>(_items, _indexSpecification);
            }
            //else if (e.Action == NotifyCollectionChangedAction.Move)
            //{
            //    //No action should be needed on a move as nothing is removed or added...
            //}
        }

        public IIndex<T> GetIndexByPropertyName(string propertyName)
        {
            return _internalIndexableCollection.GetIndexByPropertyName(propertyName);
        }

        public bool ContainsIndex(string propertyName)
        {
            return _internalIndexableCollection.ContainsIndex(propertyName);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalIndexableCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}