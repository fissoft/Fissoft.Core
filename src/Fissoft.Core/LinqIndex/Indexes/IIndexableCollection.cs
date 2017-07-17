using System.Collections.Generic;

namespace Fissoft.LinqIndex.Indexes
{
    public interface IIndexableCollection<T> : IEnumerable<T>
    {
        IIndex<T> GetIndexByPropertyName(string propertyName);
        bool ContainsIndex(string propertyName);
    }
}