using System;
using System.Reflection;

namespace Fissoft.LinqIndex.Internal
{
    internal class PropertyReader<T>
    {
        private readonly PropertyReadStrategy _propertyReadStrategy;
        private LateBoundProperty _propertyValueReadProperty;
        private readonly PropertyInfo _propertyInfo;

        private LateBoundProperty PropertyValueReadProperty
        {
            get
            {
                // this property is "late-bound" because if this reader is never 
                // accessed, there's no need to expose the up front reflection cost
                return (_propertyValueReadProperty ??
                        (_propertyValueReadProperty = DelegateFactory.Create<T>(_propertyInfo)));
            }
        }

        public string PropertyName { get { return _propertyInfo.Name; } }

        public PropertyReader(IndexPropertySpecification indexPropertySpecification)
        {
            if (indexPropertySpecification == null)
                throw new ArgumentNullException("indexPropertySpecification");

            _propertyReadStrategy = indexPropertySpecification.PropertyReadStrategy;

            var propertyName = indexPropertySpecification.PropertyName;

            var typeOfT = typeof(T);

            _propertyInfo = typeOfT.GetTypeInfo().GetProperty(propertyName);

            if (_propertyInfo == null)
                throw new ArgumentException("Could not find property name [{0}] on type [{1}]."
                    .FormatWith(propertyName, typeOfT.FullName));
        }

        public object ReadValue(T @rootObject)
        {
            if (_propertyReadStrategy == PropertyReadStrategy.DelegateMethod)
            {
                return PropertyValueReadProperty(@rootObject);
            }

            return _propertyInfo.GetValue(@rootObject, null);
        }

        /// <summary>
        /// Returns the class's configured Property's hashcode - or zero is the object is null
        /// </summary>
        public int GetItemHashCode(T @rootObject)
        {
            var value = ReadValue(@rootObject);

            if (value == null)
                return Consts.NullKeyHashCode;

            return value.GetHashCode();
        }
    }
}