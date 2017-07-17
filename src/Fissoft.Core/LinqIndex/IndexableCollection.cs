using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fissoft.Framework.Systems.Data.Indexes;
using Fissoft.Framework.Systems.Data.Internal;

namespace Fissoft.Framework.Systems.Data
{

    /*
     * TODO: Notes next version...
     * 
     * modified external API change GetIndexByPropertyName to internal & changed return type
     * 
     */

    public class IndexableCollection<T> : IList<T>, IIndexableCollection<T>
    {
        private readonly List<T> _internalList = new List<T>();
        private readonly InternalIndexCollection<T> _indexs = new InternalIndexCollection<T>();


        public IndexableCollection()
            : this(new List<T>())
        {
        }

        public IndexableCollection(IEnumerable<T> items)
            : this(items, new IndexSpecification<T>())
        {
        }

        public IndexableCollection(IndexSpecification<T> indexSpecification)
            : this(new List<T>(), indexSpecification)
        { }

        public IndexableCollection(IEnumerable<T> items, IndexSpecification<T> indexSpecification)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (indexSpecification == null)
                throw new ArgumentNullException("indexSpecification");

            UseIndexSpecification(indexSpecification);

            foreach (var item in items)
                Add(item);
        }

        public IndexableCollection<T> CreateIndexFor<TParameter>(Expression<Func<T, TParameter>> propertyExpression)
        {
            return CreateIndexFor(propertyExpression, Consts.DefaultPropertyReadStrategy);
        }

        public IndexableCollection<T> CreateIndexFor<TParameter>(Expression<Func<T, TParameter>> propertyExpression, PropertyReadStrategy propertyReadStrategy)
        {
            var propertyName = propertyExpression.GetMemberName();
            var propertyConfiguration = new IndexPropertySpecification(propertyName, propertyReadStrategy);

            return CreateIndexFor(propertyConfiguration);
        }

        public bool RemoveIndexFor<TParameter>(Expression<Func<T, TParameter>> propertyExpression)
        {
            var propertyName = propertyExpression.GetMemberName();

            if (_indexs.ContainsIndex(propertyName))
                return _indexs.RemoveIndex(propertyName);

            return false;
        }

        public bool ContainsIndex<TParameter>(Expression<Func<T, TParameter>> propertyExpression)
        {
            return ContainsIndex(propertyExpression.GetMemberName());
        }

        public bool ContainsIndex(string propertyName)
        {
            return _indexs.ContainsIndex(propertyName);
        }

        public void Add(T item)
        {
            AddItemToIndexes(item);

            _internalList.Add(item);
        }

        public bool Remove(T item)
        {
            RemoveItemFromIndexes(item);

            return _internalList.Remove(item);
        }


        public IndexableCollection<T> UseIndexSpecification(IndexSpecification<T> indexSpecification)
        {
            if (indexSpecification == null)
                throw new ArgumentNullException("indexSpecification");

            _indexs.Clear();

            foreach (var property in indexSpecification.IndexedPropertiesConfiguration)
            {
                CreateIndexFor(property);
            }

            return this;
        }

        private IndexableCollection<T> CreateIndexFor(IndexPropertySpecification indexPropertySpecification)
        {
            var newIndex = new DictionaryHashIndex<T>(indexPropertySpecification, _internalList);

            _indexs.AddIndexFor(indexPropertySpecification, newIndex);

            return this;
        }

        #region ICollection<T> Members


        public void Clear()
        {
            while (_internalList.Count > 0)
                RemoveAt(0);
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_internalList).GetEnumerator();
        }

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            AddItemToIndexes(item);

            _internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            RemoveItemFromIndexes(this[index]);

            _internalList.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _internalList[index];
            }
            set
            {
                RemoveItemFromIndexes(_internalList[index]);
                _internalList[index] = value;
                AddItemToIndexes(_internalList[index]);
            }
        }
        #endregion


        private void AddItemToIndexes(T item)
        {
            foreach (var index in _indexs)
            {
                index.AddToIndex(item);
            }
        }

        private void RemoveItemFromIndexes(T item)
        {
            foreach (var index in _indexs)
            {
                index.RemoveFromIndex(item);
            }
        }

        public IIndex<T> GetIndexByPropertyName(string propertyName)
        {
            return _indexs.GetIndexByPropertyName(propertyName);
        }
    }
}
