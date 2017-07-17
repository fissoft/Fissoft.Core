using System.Collections.Generic;

namespace Fissoft.Framework.Systems.Data.Indexes
{
    public interface IIndexableCollection<T> : IEnumerable<T>
    {
        IIndex<T> GetIndexByPropertyName(string propertyName);
        bool ContainsIndex(string propertyName);
    }
}