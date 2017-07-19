using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Fissoft
{
    public static class TypeExtensions
    {
        public static readonly Type[] PredefinedTypes =
        {
            typeof(object),
            typeof(bool),
            typeof(char),
            typeof(string),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        public static bool IsPredefinedType(this Type type)
        {
            foreach (var t in PredefinedTypes)
                if (t == type)
                    return true;
            return false;
        }

        public static string FirstSortableProperty(this Type type)
        {
            var firstSortableProperty = type.GetTypeInfo().GetProperties()
                .Where(property => property.PropertyType.IsPredefinedType()).FirstOrDefault();

            if (firstSortableProperty == null)
                throw new NotSupportedException("CannotFindPropertyToSortBy");

            return firstSortableProperty.Name;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetTypeInfo().GetGenericArguments()[0] : type;
        }

        public static string GetTypeName(this Type type)
        {
            var baseType = GetNonNullableType(type);
            var s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        public static bool IsNumericType(this Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        public static bool IsSignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        public static bool IsUnsignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        public static int GetNumericTypeKind(this Type type)
        {
            if (type == null)
                return 0;

            type = GetNonNullableType(type);

            if (type.GetTypeInfo().IsEnum)
                return 0;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        public static PropertyInfo GetIndexerPropertyInfo(this Type type, params Type[] indexerArguments)
        {
            return
            (from p in type.GetTypeInfo().GetProperties()
                where AreArgumentsApplicable(indexerArguments, p.GetIndexParameters())
                select p).FirstOrDefault();
        }

        private static bool AreArgumentsApplicable(IEnumerable<Type> arguments, IEnumerable<ParameterInfo> parameters)
        {
            var argumentList = arguments.ToList();
            var parameterList = parameters.ToList();

            if (argumentList.Count != parameterList.Count)
                return false;

            for (var i = 0; i < argumentList.Count; i++)
                if (parameterList[i].ParameterType != argumentList[i])
                    return false;

            return true;
        }

        public static bool IsEnumType(this Type type)
        {
            return GetNonNullableType(type).GetTypeInfo().IsEnum;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static bool IsCompatibleWith(this Type source, Type target)
        {
            if (source == target) return true;
            if (!target.GetTypeInfo().IsValueType)
                return target.GetTypeInfo().IsAssignableFrom(source);
            var st = source.GetNonNullableType();
            Type tt = target.GetNonNullableType();
            if (st != source && tt == target) return false;
            var sc = st.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            var tc = tt.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        public static Type FindGenericType(this Type type, Type genericType)
        {
            var typeInfo = type.GetTypeInfo();
            var genericTypeInfo = genericType.GetTypeInfo();
            while (type != null && type != typeof(object))
            {
                if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == genericType) return type;
                if (genericTypeInfo.IsInterface)
                    foreach (var intfType in typeInfo.GetInterfaces())
                    {
                        Type found = intfType.FindGenericType(genericType);
                        if (found != null) return found;
                    }
                type = typeInfo.BaseType;
            }
            return null;
        }

        public static string GetName(this Type type)
        {
            return type.FullName.Replace(type.Namespace + ".", "");
        }

        public static object DefaultValue(this Type type)
        {
            if (type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static MemberInfo FindPropertyOrField(this Type type, string memberName)
        {
            var memberInfo = type.FindPropertyOrField(memberName, false);

            if (memberInfo == null)
                memberInfo = type.FindPropertyOrField(memberName, true);

            return memberInfo;
        }

        public static MemberInfo FindPropertyOrField(this Type type, string memberName, bool staticAccess)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                        (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in type.SelfAndBaseTypes())
            {
                var members = t.GetTypeInfo().GetMembers(flags)
                    .Where(c => c.Name == memberName).ToArray();
                // MemberTypes.Property | MemberTypes.Field,
                //flags,
                //   Type.FilterNameIgnoreCase,
                //    memberName
                if (members.Length != 0) return members[0];
            }
            return null;
        }


        public static IEnumerable<Type> SelfAndBaseTypes(this Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        public static IEnumerable<Type> SelfAndBaseClasses(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (var t in type.GetTypeInfo().GetInterfaces()) AddInterface(types, t);
            }
        }

        //public static bool IsDataRow(this Type type)
        //{
        //    return type.IsCompatibleWith(typeof(DataRow)) || type.IsCompatibleWith(typeof(DataRowView));
        //}

        public static bool IsDynamicObject(this Type type)
        {
            return type == typeof(object) || type.IsCompatibleWith(typeof(IDynamicMetaObjectProvider));
        }

        public static bool IsDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public static string ToJavaScriptType(this Type type)
        {
            if (type == null)
                return "Object";

            if (type == typeof(char) || type == typeof(char?))
                return "String";

            if (IsNumericType(type))
                return "Number";

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return "Date";

            if (type == typeof(string))
                return "String";

            if (type == typeof(bool) || type == typeof(bool?))
                return "Boolean";

            if (type.GetTypeInfo().IsEnum)
                return "Enum";

            return "Object";
        }
        //}
        //        !(type.IsCompatibleWith(typeof(ICustomTypeDescriptor)));

        //        !type.IsDataRow() &&


        //        !type.IsDynamicObject() &&
        //    return
        //{

        //public static bool IsPlainType(this Type type)
    }
}