using System.Collections.Generic;
using System.ComponentModel;
using Fissoft.LinqIndex.Internal;

namespace Fissoft.LinqIndex.Indexes
{

    internal class ObservableDictionaryHashIndex<T> : DictionaryHashIndex<T>
    {
        /// <summary>
        /// local field to store single instance of the handler for speed purposes
        /// </summary>
        private readonly PropertyChangedEventHandler _propertyChangedHandler;

        public ObservableDictionaryHashIndex(string propertyName, IEnumerable<T> items)
            : this(new IndexPropertySpecification(propertyName), items)
        { }

        public ObservableDictionaryHashIndex(IndexPropertySpecification indexPropertySpecification)
            : this(indexPropertySpecification, new List<T>())
        { }

        public ObservableDictionaryHashIndex(IndexPropertySpecification indexPropertySpecification, IEnumerable<T> items)
            : base(indexPropertySpecification, items)
        {
            _propertyChangedHandler = IndexableCollection_PropertyChanged;
        }

        public override void AddToIndex(T item)
        {
            base.AddToIndex(item);

            WireNotifyPropertyChangeEvents(item);
        }

        public override void RemoveFromIndex(T item)
        {
            base.RemoveFromIndex(item);

            UnWireNotifyPropertyChangeEvents(item);
        }

        private void WireNotifyPropertyChangeEvents(object item)
        {
            ((INotifyPropertyChanged)item).PropertyChanged += _propertyChangedHandler;
        }

        private void UnWireNotifyPropertyChangeEvents(object item)
        {
            ((INotifyPropertyChanged)item).PropertyChanged -= _propertyChangedHandler;
        }

        /// <summary>
        /// Adds an indexed property to the index after it has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IndexableCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!string.Equals(PropertyReader.PropertyName, e.PropertyName))
                return;

            RemoveFromIndex((T)sender);
            AddToIndex((T)sender);
        }
    }

}