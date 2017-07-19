using System.Collections.Generic;
using System.Collections.Specialized;
using Fissoft.LinqIndex.Internal;

namespace Fissoft.LinqIndex
{
    public static class ObservablesMonitor
    {
        public static void BeginObserving<TCollection, T>(TCollection observableCollection,
            IndexSpecification<T> indexSpecification)
            where TCollection : class, INotifyCollectionChanged, IEnumerable<T>
        {
            InternalObservablesHook.Add(observableCollection, indexSpecification);
        }

        //// TODO:???? Investigate this spiked idea...
        //public static void BeginObservingAsync<TCollection, T>(TCollection observableCollection, IndexSpecification<T> indexSpecification, Action<TCollection> finishedBuildingCollectionCallback)
        //    where TCollection : class, INotifyCollectionChanged, IEnumerable<T>
        //{
        //    //TODO Start up background worker. And add the collection to be monitored?
        //    //TODO then call the callback handler???
        //    var indexWorker = new BackgroundWorker();
        //    indexWorker.DoWork += (sender, e) =>
        //                              {
        //                                  InternalObservablesHook.Add(observableCollection, indexSpecification);
        //                                  //TODO: think about thread marshaling?? UI Thread???
        //                                  finishedBuildingCollectionCallback(observableCollection);
        //                              };
        //}


        public static void ClearAll()
        {
            InternalObservablesHook.ItemsMonitoring.Clear();
        }

        public static bool IsUnderObservation(object collection)
        {
            return InternalObservablesHook.ContainsCollection(collection);
        }
    }
}