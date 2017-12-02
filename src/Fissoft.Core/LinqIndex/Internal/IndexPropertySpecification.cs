using System;

namespace Fissoft.LinqIndex.Internal
{
    internal class IndexPropertySpecification
    {
        public IndexPropertySpecification(string propertyName)
            : this(propertyName, Consts.DefaultPropertyReadStrategy)
        {
        }

        public IndexPropertySpecification(string propertyName, PropertyReadStrategy propertyReadStrategy)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyName = propertyName;
            PropertyReadStrategy = propertyReadStrategy;
        }

        public string PropertyName { get; }
        public PropertyReadStrategy PropertyReadStrategy { get; }

        #region Equals & operator ==

        public bool Equals(IndexPropertySpecification other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.PropertyName, PropertyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(IndexPropertySpecification)) return false;
            return Equals((IndexPropertySpecification) obj);
        }

        public override int GetHashCode()
        {
            return PropertyName != null ? PropertyName.GetHashCode() : 0;
        }

        public static bool operator ==(IndexPropertySpecification a, IndexPropertySpecification b)
        {
            if ((object) a == null && (object) b == null)
            {
                Console.WriteLine("x");
                return true;
            }

            if ((object) a == null || (object) b == null)
                return false;

            return a.PropertyName == b.PropertyName;
        }

        public static bool operator !=(IndexPropertySpecification a, IndexPropertySpecification b)
        {
            return !(a == b);
        }

        #endregion
    }
}