using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Fissoft.EntitySearch;
using Fissoft.Framework.Systems.Common;
using Fissoft.Framework.Systems.Data.EntitySearch;

namespace Fissoft
{
    /// <summary>
    ///   Linq IQueryable 扩展
    /// </summary>
    public static class IQueryableExtensions
    {
        #region OrderBy

        /// <summary>
        ///   扩展OrderBy方法，使之支持字符串
        ///   Ganji.Framework.Systems.IQueryableExtensions
        ///   支持多字段组合排序，如
        ///   OrderBy("P1,P2")
        ///   OrderBy("P1,P2","desc")
        ///   OrderBy("P1,P2",",desc")
        ///   OrderBy("P1,P2","desc,asc")
        ///   OrderBy("P1,P2","asc,asc")
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "query"></param>
        /// <param name = "sortName"></param>
        /// <param name = "sortOrder"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortName, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortName)) return query;
            var retQuery = query;
            var propGroup = sortName.Split(',');

            for (int k = 0; k < propGroup.Length; k++)
            {
                var typeOfProp = typeof(T);
                var sortField = propGroup[k];
                var currentOrder = GetOrderMethod(sortOrder, k);
                var param = Expression.Parameter(typeof(T), "o");
                var props = sortField.Split('.');
                Expression propertyAccess = param;
                var i = 0;
                do
                {
                    var property = ReflectionTypePropertyCache<T>.GetProperty(props[i]);
                    if (property == null) throw new Exception("OrderBy方法,可能位于Where(SearchModel)中，或不到所指定的属性:" + sortName);
                    typeOfProp = property.PropertyType;
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                    i++;
                } while (i < props.Length);

                var orderByExp = Expression.Lambda(propertyAccess, param);
                var resultExp = Expression.Call(typeof(Queryable),
                        currentOrder,
                        new[] { typeof(T), typeOfProp },
                        retQuery.Expression,
                        Expression.Quote(orderByExp));
                retQuery = retQuery.Provider.CreateQuery<T>(resultExp);
            }
            return retQuery;
        }

        private static string GetOrderMethod(string sortOrder, int index)
        {
            const string ASC = "OrderBy";
            const string DESC = "OrderByDescending";
            const string THENASC = "ThenBy";
            const string THENDESC = "ThenByDescending";
            string AscVar, DescVar;
            if (index == 0)
            {
                AscVar = ASC;
                DescVar = DESC;
            }
            else
            {
                AscVar = THENASC;
                DescVar = THENDESC;
            }

            if (string.IsNullOrEmpty(sortOrder)) return AscVar;
            var orderArr = sortOrder.Split(',');
            if (index >= orderArr.Length) return AscVar;
            if (orderArr[index].ToLower() == "desc") return DescVar;
            return AscVar;
        }
        #endregion

        #region Where in

        /// <summary>
        ///   使之支持Sql in语法
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <typeparam name = "TValue"></typeparam>
        /// <param name = "query"></param>
        /// <param name = "obj"></param>
        /// <param name = "values"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIn<T, TValue>(this IQueryable<T> query, Expression<Func<T, TValue>> obj, IEnumerable<TValue> values)
        {
            return query.Where(BuildContainsExpression(obj, values));
        }

        private static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(
            Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector)
            {
                throw new ArgumentNullException("valueSelector");
            }
            if (null == values)
            {
                throw new ArgumentNullException("values");
            }
            var p = valueSelector.Parameters.Single();
            if (!values.Any()) return e => false;

            var equals = values.Select(value => (Expression) Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof (TValue))));
            var body = equals.Aggregate(Expression.Or);
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        #endregion

        #region Where

        /// <summary>
        ///   zoujian add , 使IQueryable支持SearchModel
        /// </summary>
        /// <typeparam name = "TEntity"></typeparam>
        /// <param name = "table">IQueryable的查询对象</param>
        /// <param name = "model">SearchModel对象</param>
        /// <param name = "prefix">使用前缀区分查询条件</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> table, SearchModel model, string prefix = "") where TEntity : class
        {
            Contract.Requires(table != null);
            if(!string.IsNullOrEmpty(prefix))
            {
                return new QueryableSearcher<TEntity>(table, model.Items.Where(c => c.Prefix == prefix)).Search();
            }
            return new QueryableSearcher<TEntity>(table, model.Items.Where(c => string.IsNullOrEmpty(c.Prefix))).Search();

        }

        /// <summary>
        ///   支持SearchItem查询
        /// </summary>
        /// <typeparam name = "TEntity"></typeparam>
        /// <param name = "table">IQueryable的查询对象</param>
        /// <param name = "item">SearchItem</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> table, SearchItem item)
        {
            Contract.Requires(item != null);
            Contract.Requires(!string.IsNullOrEmpty(item.Field));
            return new QueryableSearcher<TEntity>(table, new List<SearchItem> {item}).Search();
        }

        /// <summary>
        ///   支持对IQueryable通过字符串指定属性的查询
        /// </summary>
        /// <typeparam name = "TEntity"></typeparam>
        /// <param name = "table">IQueryable的查询对象</param>
        /// <param name = "field">属性名</param>
        /// <param name = "method">判断谓词</param>
        /// <param name = "value">值</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> table, string field,
                                                         SearchMethod method, object value)
        {
            return table.Where(new SearchItem {Field = field, Method = method, Value = value});
        }

        #endregion

        #region Obsolete

        [Obsolete("请使用Where方法，并设置SearchItem的OrGroup", true)]
        public static IQueryable<TEntity> WhereOr<TEntity>(IQueryable<TEntity> ret, IEnumerable<SearchItem> entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}