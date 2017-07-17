using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fissoft.LinqIndex.Internal;

namespace Fissoft.LinqIndex
{
    public class IndexSpecification<T>
    {
        private readonly List<IndexPropertySpecification> _indexedPropertiesConfiguration;

        public IndexSpecification()
        {
            _indexedPropertiesConfiguration = new List<IndexPropertySpecification>();
        }

        internal IList<IndexPropertySpecification> IndexedPropertiesConfiguration
        {
            get { return _indexedPropertiesConfiguration; }
        }

        public IEnumerable<string> IndexedProperties
        {
            get
            {
                return _indexedPropertiesConfiguration.Select(s => s.PropertyName);
            }
        }

        public IndexSpecification<T> Add<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            return Add(propertyExpressions, Consts.DefaultPropertyReadStrategy);
        }

        public IndexSpecification<T> Add<TProperty>(Expression<Func<T, TProperty>> propertyExpressions, PropertyReadStrategy propertyReadStrategy)
        {
            var propertyName = propertyExpressions.GetMemberName();
            return Add(new IndexPropertySpecification(propertyName, propertyReadStrategy));
        }

        private IndexSpecification<T> Add(IndexPropertySpecification propertySpecification)
        {
            // Should only add property once
            if (!IndexedPropertiesConfiguration.Any(i => i == propertySpecification))
                IndexedPropertiesConfiguration.Add(propertySpecification);

            return this;
        }

        public IndexSpecification<T> Remove<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var propertyName = propertyExpressions.GetMemberName();

            var itemToRemove = IndexedPropertiesConfiguration
                .Where(w => w.PropertyName.Equals(propertyName))
                .FirstOrDefault();

            if (itemToRemove != null)
                IndexedPropertiesConfiguration.Remove(itemToRemove);

            return this;
        }
    }
}
