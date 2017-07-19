using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Fissoft.EntitySearch;

namespace Fissoft
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     解决foreach时可能数据为null的问题
        ///     <example>
        ///         foreach(var item in list.ToNotNull()){ /* ... */ }
        ///     </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static List<T> ToNotNull<T>(this IEnumerable<T> ie)
        {
            return ie != null ? ie.ToList() : new List<T>();
        }

        /// <summary>
        ///     将集合转换为只读集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            var readOnlyCollection = source as ReadOnlyCollection<T>;
            if (readOnlyCollection != null)
                return readOnlyCollection;
            return new ReadOnlyCollection<T>(source.ToList());
        }

        public static List<object> ToObjectList(this IEnumerable source)
        {
            var list = new List<object>();
            var enumberable = source.GetEnumerator();
            do
            {
                list.Add(enumberable.Current);
            } while (enumberable.MoveNext());
            return list;
        }

        /// <summary>
        ///     返回元素集合的拼接字符串形式
        ///     {'dsfs','121','ds3f'} =>dsfs,121,ds3f
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPoperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="joinChar"></param>
        /// <returns></returns>
        public static string ToJoinString<T, TPoperty>(this IEnumerable<T> source, Func<T, TPoperty> selector,
            char joinChar = ',')
        {
            var builder = new StringBuilder();
            foreach (var item in source)
            {
                var poperty = selector(item);
                builder.Append(poperty);
                builder.Append(joinChar);
            }
            if (builder.Length > 0)
                builder = builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        /// <summary>
        ///     返回元素集合的拼接字符串形式
        ///     {'dsfs','121','ds3f'} =>,dsfs,121,ds3f,
        ///     主要用来 兼容 字段的 like 语句，查询方式 可以采用 like %,n,%
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPoperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="joinChar"></param>
        /// <returns></returns>
        public static string ToLikingString<T, TPoperty>(this IEnumerable<T> source, Func<T, TPoperty> selector,
            char joinChar = ',')
        {
            var builder = new StringBuilder(joinChar.ToString());
            foreach (var item in source)
            {
                var poperty = selector(item);
                builder.Append(poperty);
                builder.Append(joinChar);
            }
            if (builder.Length <= 1)
                return string.Empty;
            return builder.ToString();
        }

        /// <summary>
        ///     返回元素集合的拼接字符串形式
        ///     {'dsfs','121','ds3f'} =>dsfs,121,ds3f
        /// </summary>
        /// <param name="source"></param>
        /// <param name="joinChar"></param>
        /// <returns></returns>
        public static string ToJoinString<T>(this IEnumerable<T> source, char joinChar = ',')
        {
            var builder = new StringBuilder();
            foreach (var item in source)
            {
                builder.Append(item);
                builder.Append(joinChar);
            }
            if (builder.Length > 0)
                builder = builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static string ToJoinString<T>(this IEnumerable<T> source, string joinStr)
        {
            var builder = new StringBuilder();
            foreach (var item in source)
            {
                builder.Append(item);
                builder.Append(joinStr);
            }
            if (builder.Length > 0)
                builder = builder.Remove(builder.Length - joinStr.Length, joinStr.Length);
            return builder.ToString();
        }

        /// <summary>
        ///     返回元素集合的拼接字符串形式
        ///     {'dsfs','121','ds3f'} =>,dsfs,121,ds3f,
        ///     主要用来 兼容 字段的 like 语句，查询方式 可以采用 like %,n,%
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="joinChar"></param>
        /// <returns></returns>
        public static string ToLikingString<T>(this IEnumerable<T> source, char joinChar = ',')
        {
            var builder = new StringBuilder(joinChar.ToString());
            foreach (var item in source)
            {
                builder.Append(item);
                builder.Append(joinChar);
            }
            if (builder.Length <= 1)
                return string.Empty;
            return builder.ToString();
        }

        public static List<string> ToJoinString<T>(this IEnumerable<T> source, int groupNum, char joinChar = ',')
        {
            var list = new List<string>();
            var builder = new StringBuilder();
            var count = 0;
            foreach (var item in source)
            {
                count++;
                builder.Append(item);
                builder.Append(joinChar);
                if (count == groupNum)
                {
                    if (builder.Length > 0)
                        builder = builder.Remove(builder.Length - 1, 1);
                    list.Add(builder.ToString());
                    builder.Clear();
                    count = 0;
                }
            }
            if (builder.Length > 0)
            {
                builder = builder.Remove(builder.Length - 1, 1);
                list.Add(builder.ToString());
            }
            return list;
        }

        public static List<string> ToJoinString<T, TPoperty>(this IEnumerable<T> source, Func<T, TPoperty> selector,
            int groupNum, char joinChar = ',', bool addTail = true)
        {
            var list = new List<string>();
            var builder = new StringBuilder();
            var count = 0;
            foreach (var item in source)
            {
                count++;
                var poperty = selector(item);
                builder.Append(poperty);
                builder.Append(joinChar);
                if (count == groupNum)
                {
                    if (builder.Length > 0)
                        builder = builder.Remove(builder.Length - 1, 1);
                    list.Add(builder.ToString());
                    builder.Clear();
                    count = 0;
                }
            }
            if (addTail && builder.Length > 0)
            {
                builder = builder.Remove(builder.Length - 1, 1);
                list.Add(builder.ToString());
            }
            return list;
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, SearchItem searchItem)
        {
            var info = typeof(T).GetTypeInfo().GetProperty(searchItem.Field);
            Func<T, bool> func = delegate(T m) { return searchItem.IsMatch(info.GetValue(m, null)); };
            return source.Where(func);
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, SearchModel searchModel)
        {
            foreach (var searchItem in searchModel.Items)
            {
                source = source.Where(searchItem);
                if (source.FirstOrDefault() == null)
                    break;
            }
            return source;
        }

        public static Func<T, Tkey> MehodLambda<T, Tkey>(string propertyName)
        {
            var p = Expression.Parameter(typeof(T), "p");
            Expression body = Expression.Property(p, typeof(T).GetTypeInfo().GetProperty(propertyName));
            var lambda = Expression.Lambda<Func<T, Tkey>>(body, p);
            return lambda.Compile();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyStr, bool isDesc = false)
            where T : class
        {
            if (string.IsNullOrEmpty(propertyStr)) return source;
            var param = Expression.Parameter(typeof(T), "c");
            var property = typeof(T).GetTypeInfo().GetProperty(propertyStr);
            Expression propertyAccessExpression = Expression.MakeMemberAccess(param, property);
            var le = Expression.Lambda(propertyAccessExpression, param);
            var type = typeof(T);
            var mehodName = "OrderBy";
            if (isDesc)
                mehodName = "OrderByDescending";
            var resultExp = Expression.Call(typeof(Queryable), mehodName, new[] {type, property.PropertyType},
                source.Expression, Expression.Quote(le));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static PagedList<T> WhereToPagedList<T>(this IEnumerable<T> source, SearchModel searchModel)
            where T : class
        {
            var isDesc = searchModel.SortOrder != null && searchModel.SortOrder.ToLower() == "desc";
            IQueryable<T> resultList = source.AsQueryable().OrderBy(searchModel.SortName, isDesc).Where(searchModel);
            return new PagedList<T>(resultList, searchModel.Page, searchModel.PageSize);
        }

        /// <summary>
        ///     Source元素 与 any 是否有交集
        ///     即 Source元素是否包含any元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="any"></param>
        /// <returns></returns>
        public static bool HasAny<T>(this IEnumerable<T> source, IEnumerable<T> any)
        {
            foreach (var item in source)
                if (any.Contains(item))
                    return true;
            return false;
        }

        /// <summary>
        ///     All 是否是 Source的子集
        ///     退出 All是否全部包含在Source元素集中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static bool HasAll<T>(this IEnumerable<T> source, IEnumerable<T> all)
        {
            foreach (var item in all)
                if (!source.Contains(item))
                    return false;
            return true;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }
    }
}