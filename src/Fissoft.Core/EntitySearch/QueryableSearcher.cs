using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fissoft.Internal;
using Fissoft.Transforms;
using Fissoft.Utils;
// ReSharper disable StaticMemberInGenericType

namespace Fissoft.EntitySearch
{
    internal class QueryableSearcher<T>
    {
        private QueryProviderType _queryProviderType = QueryProviderType.Unknow;

        public QueryableSearcher()
        {
            Init();
        }

        public QueryableSearcher(IQueryable<T> table, IEnumerable<SearchItem> items)
            : this()
        {
            Table = table;
            Items = items;
        }

        public List<ITransformProvider> TransformProviders { get; set; }
        protected IEnumerable<SearchItem> Items { get; set; }

        protected IQueryable<T> Table { get; set; }

        private QueryProviderType ProviderType
        {
            get
            {
                if (_queryProviderType != QueryProviderType.Unknow) return _queryProviderType;
                var provider = Table.Provider;
                var providerType = provider.GetType().FullName;
                if (string.IsNullOrWhiteSpace(providerType))
                    return _queryProviderType = QueryProviderType.Unknow;
                if (providerType.StartsWith("System.Linq.EnumerableQuery"))
                    return _queryProviderType = QueryProviderType.EnumerableQuery;
                if (providerType.StartsWith("System.Data.Entity.Internal.Linq.DbQueryProvider"))
                    return _queryProviderType = QueryProviderType.DbQueryProvider;
                return QueryProviderType.Unknow;
            }
        }

        private void Init()
        {
            //初始化默认转换器
            TransformProviders = new List<ITransformProvider>
            {
                new LikeTransformProvider(),
                new DateBlockTransformProvider(),
                new InTransformProvider(),
                //new UnixTimeTransformProvider(),
                new NotInTransformProvider()
            };
        }

        public IQueryable<T> Search()
        {
            var param = Expression.Parameter(typeof(T), "c"); //c=>
            //获取lambda表达式方法体
            var body = GetAllExpression(param, Items);
            if (body == null) return Table;

            return Table.Where(Expression.Lambda<Func<T, bool>>(body, param));
        }

        //获取整个方法体
        private Expression GetAllExpression(ParameterExpression param, IEnumerable<SearchItem> items)
        {
            var list = new List<Expression>();
            //先处理无OrGroup的数据并使用And逻辑拼接
            var searchItems = items as SearchItem[] ?? items.ToArray();
            var andList = searchItems.Where(c => string.IsNullOrEmpty(c.OrGroup)).ToArray();
            if (andList.Length != 0)
                list.Add(GetGroupExpression(param, andList, Expression.AndAlso));
            //处理有OrGroup的条件，并按OrGroup分组后使用OR拼接
            var orGroupByList = searchItems.Where(c => !string.IsNullOrEmpty(c.OrGroup)).GroupBy(c => c.OrGroup);
            foreach (var group in orGroupByList)
                if (group.Count() != 0)
                    list.Add(GetGroupExpression(param, group, Expression.OrElse));
            if (list.Count == 0) return null;
            return list.Aggregate(Expression.AndAlso);
        }

        //按照func的规则联合 各个表达式
        private Expression GetGroupExpression(ParameterExpression param, IEnumerable<SearchItem> items,
            Func<Expression, Expression, Expression> func)
        {
            var list = items.Select(item => GetExpression(param, item));
            return list.Aggregate(func);
        }

        //获取逻辑表达式如 id==1
        private Expression GetExpression(ParameterExpression param, SearchItem item)
        {
            //属性表达式
            var exp = GetPropertyLambdaExpression(item, param);
            foreach (var provider in TransformProviders)
                if (provider.Match(item, exp.Body.Type))
                    return GetGroupExpression(param, provider.Transform(item, exp.Body.Type), Expression.AndAlso);
            //常量表达式
            var constant = ChangeTypeToExpression(item, exp.Body.Type);
            //使用已有谓词关联

            var expression = GetProviderByDiffProvider(item.Method);
            return expression(exp.Body, constant);
        }

        private Func<Expression, Expression, Expression> GetProviderByDiffProvider(SearchMethod method)
        {
            var providerType = ProviderType;
            if (providerType == QueryProviderType.EnumerableQuery)
                if (EnumerableExpressionDict.ContainsKey(method))
                    return EnumerableExpressionDict[method];
            if (providerType == QueryProviderType.DbQueryProvider)
                if (DbExpressionDict.ContainsKey(method))
                    return DbExpressionDict[method];
            return CommonExpressionDict[method];
        }

        //获取属性
        private LambdaExpression GetPropertyLambdaExpression(SearchItem item, ParameterExpression param)
        {
            var props = item.Field.Split('.');
            Expression propertyAccess = param;
            // ReSharper disable once NotAccessedVariable
            var typeOfProp = typeof(T);
            //由于属性可能是多层，所以提供了  Prop.Prop1.Prop2这样的功能，来进行多层查询
            var i = 0;
            do
            {
                var property = ReflectionTypePropertyCache<T>.GetProperty(props[i]);
                if (property == null)
                    throw new Exception(
                        string.Format("{0}中的属性{1}不存在，所以不能用于查询，请检查查询条件",
                            typeof(T), props[i]
                        ));
                // ReSharper disable once RedundantAssignment
                typeOfProp = property.PropertyType;
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                i++;
            } while (i < props.Length);

            return Expression.Lambda(propertyAccess, param);
        }

        #region ChangeType

        /// <summary>
        ///     类型转换，支持非空类型与可空类型之间的转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;
            return Convert.ChangeType(value, TypeUtil.GetUnNullableType(conversionType));
        }

        /// <summary>
        ///     转换SearchItem中的Value的类型，为表达式树
        /// </summary>
        /// <param name="item"></param>
        /// <param name="conversionType">目标类型</param>
        public static Expression ChangeTypeToExpression(SearchItem item, Type conversionType)
        {
            if (item.Value == null) return Expression.Constant(item.Value, conversionType);
            
            #region 数组
            if (item.Value is IList arr && !(arr is IEnumerable<char>))
            {
                var expList = new List<Expression>();
                foreach (object val in arr)
                {
//构造数组的单元Constant
                    var newValue = ChangeType(val, conversionType);
                    expList.Add(Expression.Constant(newValue, conversionType));
                }
                //构造inType类型的数组表达式树，并为数组赋初值
                return Expression.NewArrayInit(conversionType, expList);
            }

            #endregion

            var elementType = TypeUtil.GetUnNullableType(conversionType);
            var value = elementType.GetTypeInfo().BaseType == typeof(Enum) ?
                Enum.Parse(elementType, item.Value.ToString()) :
                Convert.ChangeType(item.Value, elementType);
            return Expression.Constant(value, conversionType);
        }

        #endregion

        #region SearchMethod 操作方法

        private static readonly Dictionary<SearchMethod, Func<Expression, Expression, Expression>>
            EnumerableExpressionDict =
                new Dictionary<SearchMethod, Func<Expression, Expression, Expression>>
                {
                    {
                        SearchMethod.Contains,
                        (left, right) =>
                        {
                            if (left.Type != typeof(string)) return null;
                            return Expression.Condition(
                                Expression.Equal(left, Expression.Constant(null)),
                                Expression.Call(Expression.Constant(string.Empty),
                                    ReflectionStringMethodCache.Contains, right),
                                Expression.Call(left, ReflectionStringMethodCache.Contains, right)
                            );
                        }
                    }
                };


        private static readonly Dictionary<SearchMethod, Func<Expression, Expression, Expression>>
            DbExpressionDict =
                new Dictionary<SearchMethod, Func<Expression, Expression, Expression>>();

        private static readonly Dictionary<SearchMethod, Func<Expression, Expression, Expression>> CommonExpressionDict
            =
            new Dictionary<SearchMethod, Func<Expression, Expression, Expression>>
            {
                {
                    SearchMethod.Equal,
                    Expression.Equal
                },
                {
                    SearchMethod.GreaterThan,
                    Expression.GreaterThan
                },
                {
                    SearchMethod.GreaterThanOrEqual,
                    Expression.GreaterThanOrEqual
                },
                {
                    SearchMethod.LessThan,
                    Expression.LessThan
                },
                {
                    SearchMethod.LessThanOrEqual,
                    Expression.LessThanOrEqual
                },
                {
                    SearchMethod.Contains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string)) return null;
                        return Expression.Call(left, ReflectionStringMethodCache.Contains, right);
                    }
                },
                {
                    SearchMethod.StdIn,
                    (left, right) =>
                    {
                        if (!right.Type.IsArray) return null;
                        //调用Enumerable.Contains扩展方法
                        var resultExp =
                            Expression.Call(
                                typeof(Enumerable),
                                "Contains",
                                new[] {left.Type},
                                right,
                                left);

                        return resultExp;
                    }
                },
                {
                    SearchMethod.NotContains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string)) return null;
                        var resultExp = Expression.Call(left, ReflectionStringMethodCache.Contains, right);

                        return Expression.Not(resultExp);
                    }
                },

                {
                    SearchMethod.StdNotIn,
                    (left, right) =>
                    {
                        if (!right.Type.IsArray) return null;
                        //调用Enumerable.Contains扩展方法
                        var resultExp =
                            Expression.Call(
                                typeof(Enumerable),
                                "Contains",
                                new[] {left.Type},
                                right,
                                left);
                        return Expression.Not(resultExp);
                    }
                },
                {
                    SearchMethod.NotEqual,
                    Expression.NotEqual
                },
                {
                    SearchMethod.StartsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string)) return null;
                        return Expression.Call(left, ReflectionStringMethodCache.StartsWith, right);
                    }
                },
                {
                    SearchMethod.EndsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string)) return null;
                        return Expression.Call(left, ReflectionStringMethodCache.EndsWith, right);
                    }
                },
                {
                    SearchMethod.DateTimeLessThanOrEqual,
                    Expression.LessThanOrEqual
                },
                {
                    SearchMethod.IsNull,
                    (left, right) =>
                    {
                        if (right is ConstantExpression)
                        {
                            var rightCon = right as ConstantExpression;
                            var rightConValue = rightCon.Value == null ? string.Empty : rightCon.Value.ToString();
                            if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                                return Expression.Equal(left, Expression.Constant(null));
                            return Expression.NotEqual(left, Expression.Constant(null));
                        }
                        var resultExp = Expression.Or(
                            Expression.Or(Expression.Equal(right, Expression.Constant("True")),
                                Expression.Equal(right, Expression.Constant("1"))),
                            Expression.Equal(right, Expression.Constant("true")));
                        return Expression.Condition(resultExp,
                            Expression.Equal(left, Expression.Constant(null)),
                            Expression.NotEqual(left, Expression.Constant(null)));
                    }
                },
                {
                    SearchMethod.IsNullOrEmpty,
                    (left, right) =>
                    {
                        if (right is ConstantExpression )
                        {
                            var rightCon = right as ConstantExpression;
                            var rightConValue = rightCon.Value == null ? string.Empty : rightCon.Value.ToString();
                            if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                                return Expression.Or(Expression.Equal(left, Expression.Constant(null)),
                                    Expression.Equal(left, Expression.Constant("")));
                            return Expression.And(Expression.NotEqual(left, Expression.Constant(null)),
                                Expression.NotEqual(left, Expression.Constant("")));
                        }
                        var resultExp = Expression.Or(
                            Expression.Or(Expression.Equal(right, Expression.Constant("True")),
                                Expression.Equal(right, Expression.Constant("1"))),
                            Expression.Equal(right, Expression.Constant("true")));
                        return Expression.Condition(resultExp,
                            Expression.Or(Expression.Equal(left, Expression.Constant(null)),
                                Expression.Equal(left, Expression.Constant(""))),
                            Expression.And(Expression.NotEqual(left, Expression.Constant(null)),
                                Expression.NotEqual(left, Expression.Constant(""))));
                    }
                }
            };

        #endregion
    }
}