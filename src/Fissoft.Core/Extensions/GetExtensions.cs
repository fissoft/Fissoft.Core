namespace Fissoft.Framework.Systems
{
    using System;
    using System.Collections.Generic;

    public static class ClassExtensions
    {
        /// <summary>
        /// 实现如果为空则取默认值的操作 原操作obj==null?1:obj.Prop
        /// 现：obj.Get(c=>c.Prop)
        /// </summary>
        public static TResult GetProperty<TClass, TResult>(this TClass obj, Func<TClass, TResult> func) where TClass : class
        {
            if (obj == null) return default(TResult);
            return func(obj);
        }

        /// <summary>
        /// 实现如果为Key不存在，则返回Value默认值 
        /// dict.ContainsKey(key)?dict[key]:0
        /// 现在直接为 dict.Get(key)即可
        /// </summary>
        //static public TResult GetProperty<TKey, TResult>(this IDictionary<TKey, TResult> dict, TKey key)
        //{
        //    if (!dict.ContainsKey(key))
        //        return default(TResult);
        //    var obj = dict[key];
        //    return obj;
        //}
        public static TResult GetProperty<TKey, TResult>(this IReadOnlyDictionary<TKey, TResult> dict, TKey key)
        {
            if (!dict.ContainsKey(key))
                return default(TResult);
            var obj = dict[key];
            return obj;
        }
        /// <summary>
        /// 实现如果为Key不存在，则返回Value默认值 
        /// dict.ContainsKey(key)?dict[key].Id:0
        /// 现在直接为 dict.Get(key,c=>c.Id)即可
        /// </summary>
        public static TResult GetProperty<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TResult> func)
        {
            if (!dict.ContainsKey(key))
                return default(TResult);
            var obj = dict[key];
            return func(obj);
        }
    }
}