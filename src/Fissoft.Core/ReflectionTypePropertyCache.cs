using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Fissoft.Framework.Systems.Common
{
    /// <summary>
    /// GetProperty反射操作缓存
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    class ReflectionTypePropertyCache<TType>
    {
        readonly static object LockObject = new object();
        static Dictionary<string, PropertyInfo> Dict = new Dictionary<string, PropertyInfo>();
        /// <summary>
        /// 从缓存读取PropertyInfo，并存至缓存，优化反射性能
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static PropertyInfo GetProperty(string name)
        {
            if (!Dict.ContainsKey(name))
            {
                lock (LockObject)
                {
                    if (!Dict.ContainsKey(name))
                    {
                        Dict[name] = typeof(TType).GetProperty(name);
                    }
                }
            }
            return Dict[name];
        }

        internal static PropertyDescriptorCollection GetPropertiesAsDescriptor(TType obj)
        {
            return TypeDescriptor.GetProperties(obj);
        }
    }
    class ReflectionTypePropertyCache
    {
        static readonly object LockObject = new object();
        static Dictionary<Type, PropertyDescriptorCollection> Dict = new Dictionary<Type, PropertyDescriptorCollection>();

        /// <summary>
        /// 从缓存获取PropertyDescriptorCollection，及写入缓存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static PropertyDescriptorCollection GetPropertiesAsDescriptor(object obj)
        {
            var type = obj.GetType();
            if (!Dict.ContainsKey(type))
            {
                lock (LockObject)
                {
                    if (!Dict.ContainsKey(type))
                    {
                        Dict[type] = TypeDescriptor.GetProperties(obj);
                    }
                }
            }
            return Dict[type];
        }
    }
}
