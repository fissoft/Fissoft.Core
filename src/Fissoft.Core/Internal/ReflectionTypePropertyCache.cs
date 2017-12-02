using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Fissoft.Internal
{
    /// <summary>
    ///     GetProperty反射操作缓存
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal class ReflectionTypePropertyCache<TType>
    {
        private static readonly object LockObject = new object();
        private static readonly Dictionary<string, PropertyInfo> Dict = new Dictionary<string, PropertyInfo>();

        /// <summary>
        ///     从缓存读取PropertyInfo，并存至缓存，优化反射性能
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(string name)
        {
            if (!Dict.ContainsKey(name))
                lock (LockObject)
                {
                    if (!Dict.ContainsKey(name))
                        Dict[name] = typeof(TType).GetTypeInfo().GetProperty(name);
                }
            return Dict[name];
        }

        internal static PropertyDescriptorCollection GetPropertiesAsDescriptor(TType obj)
        {
            return TypeDescriptor.GetProperties(obj);
        }
    }

    internal class ReflectionTypePropertyCache
    {
        private static readonly object LockObject = new object();

        private static readonly Dictionary<Type, PropertyDescriptorCollection> Dict =
            new Dictionary<Type, PropertyDescriptorCollection>();

        /// <summary>
        ///     从缓存获取PropertyDescriptorCollection，及写入缓存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static PropertyDescriptorCollection GetPropertiesAsDescriptor(object obj)
        {
            var type = obj.GetType();
            if (!Dict.ContainsKey(type))
                lock (LockObject)
                {
                    if (!Dict.ContainsKey(type))
                        Dict[type] = TypeDescriptor.GetProperties(obj);
                }
            return Dict[type];
        }
    }
}