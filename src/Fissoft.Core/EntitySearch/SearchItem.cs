//-----------------------------------------------------------------------
// <copyright file="SearchItem" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-09-03</addtime>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fissoft.EntitySearch
{
    /// <summary>
    ///     用于存储查询条件的单元
    /// </summary>
    public class SearchItem
    {
        public SearchItem()
        {
        }

        public SearchItem(string field, SearchMethod method, object val)
        {
            Field = field;
            Method = method;
            Value = val;
            FilterId = field;
        }

        public SearchItem(string field, SearchMethod method, object val, string id)
        {
            Field = field;
            Method = method;
            Value = val;
            FilterId = id;
        }

        /// <summary>
        ///     搜索条件标记字段
        /// </summary>
        public string FilterId { get; set; }

        /// <summary>
        ///     字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        ///     查询方式，用于标记查询方式HtmlName中使用[]进行标识
        /// </summary>
        public SearchMethod Method { get; set; }

        /// <summary>
        ///     值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     前缀，用于标记作用域，HTMLName中使用()进行标识
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        ///     如果使用Or组合，则此组组合为一个Or序列
        /// </summary>
        public string OrGroup { get; set; }

        /// <summary>
        ///     查询字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        public string Description { get; set; }


        public bool IsMatch(object source)
        {
            var des = Value;
            var searchMethod = Method;
            if (source == null)
                return false;
            var sourceType = source.GetType();
            if (sourceType == typeof(string))
            {
                var sourceStr = source.ToString();
                var desStr = des.ToString();
                if (searchMethod == SearchMethod.Contains)
                    return sourceStr.Contains(desStr);
                if (searchMethod == SearchMethod.In || searchMethod == SearchMethod.StdIn)
                    return desStr.Contains(sourceStr);
                if (searchMethod == SearchMethod.Equal)
                    return sourceStr == desStr;
                if (searchMethod == SearchMethod.NotEqual)
                    return sourceStr != desStr;
                if (searchMethod == SearchMethod.EndsWith)
                    return sourceStr.EndsWith(desStr);
                if (searchMethod == SearchMethod.StartsWith)
                    return sourceStr.StartsWith(desStr);
                if (searchMethod == SearchMethod.NotIn || searchMethod == SearchMethod.StdNotIn)
                    return !desStr.Contains(sourceStr);
            }
            else if (sourceType == typeof(DateTime))
            {
                var sourceDateTime = (DateTime) source;
                var desDateTime = (DateTime) des;
                if (searchMethod == SearchMethod.Equal)
                    return sourceDateTime == desDateTime;
                if (searchMethod == SearchMethod.NotEqual)
                    return sourceDateTime != desDateTime;
                if (searchMethod == SearchMethod.DateTimeLessThanOrEqual ||
                    searchMethod == SearchMethod.LessThanOrEqual)
                    return sourceDateTime <= desDateTime;
                if (searchMethod == SearchMethod.GreaterThanOrEqual)
                    return sourceDateTime >= desDateTime;
                if (searchMethod == SearchMethod.GreaterThan)
                    return sourceDateTime > desDateTime;
                if (searchMethod == SearchMethod.LessThan)
                    return sourceDateTime < desDateTime;
            }
            else if (sourceType.GetTypeInfo().BaseType == typeof(Enum))
            {
                var sourceValue = (int) source;

                if (searchMethod == SearchMethod.Equal)
                    return sourceValue == (int) des;
                if (searchMethod == SearchMethod.NotEqual)
                    return sourceValue != (int) des;
                if (searchMethod == SearchMethod.StdIn || searchMethod == SearchMethod.In)
                {
                    var desValue = GetInValue(des);
                    return desValue.Contains(sourceValue);
                }
                if (searchMethod == SearchMethod.StdIn || searchMethod == SearchMethod.NotIn)
                {
                    var desValue = GetInValue(des);
                    return !desValue.Contains(sourceValue);
                }
            }
            else if (sourceType.GetTypeInfo().BaseType == typeof(int))
            {
                if (searchMethod == SearchMethod.Equal)
                    return (int) source == (int) des;
                if (searchMethod == SearchMethod.NotEqual)
                    return (int) source != (int) des;
                if (searchMethod == SearchMethod.DateTimeLessThanOrEqual ||
                    searchMethod == SearchMethod.LessThanOrEqual)
                    return (int) source <= (int) des;
                if (searchMethod == SearchMethod.GreaterThanOrEqual)
                    return (int) source >= (int) des;
                if (searchMethod == SearchMethod.GreaterThan)
                    return (int) source > (int) des;
                if (searchMethod == SearchMethod.LessThan)
                    return (int) source < (int) des;
                if (searchMethod == SearchMethod.StdIn || searchMethod == SearchMethod.In)
                {
                    var desValue = GetInValue(des);
                    return desValue.Contains((int) source);
                }
                if (searchMethod == SearchMethod.StdNotIn || searchMethod == SearchMethod.NotIn)
                {
                    var desValue = GetInValue(des);
                    return !desValue.Contains((int) source);
                }
            }
            else
            {
                if (searchMethod == SearchMethod.StdIn || searchMethod == SearchMethod.In)
                {
                    var desValueList = GetInValue(des);
                    return desValueList.Contains(Convert.ToInt32(source));
                }
                if (searchMethod == SearchMethod.StdNotIn || searchMethod == SearchMethod.NotIn)
                {
                    var desValueList = GetInValue(des);
                    return !desValueList.Contains(Convert.ToInt32(source));
                }

                var sourceValue = Convert.ToSingle(source);
                var desValue = Convert.ToSingle(des);
                if (searchMethod == SearchMethod.Equal)
                    return sourceValue == desValue;
                if (searchMethod == SearchMethod.NotEqual)
                    return sourceValue != desValue;
                if (searchMethod == SearchMethod.DateTimeLessThanOrEqual ||
                    searchMethod == SearchMethod.LessThanOrEqual)
                    return sourceValue <= desValue;
                if (searchMethod == SearchMethod.GreaterThanOrEqual)
                    return sourceValue >= desValue;
                if (searchMethod == SearchMethod.GreaterThan)
                    return sourceValue > desValue;
                if (searchMethod == SearchMethod.LessThan)
                    return sourceValue < desValue;
            }
            return false;
        }

        private static List<int> GetInValue(object itemvalue)
        {
            if (itemvalue is int[])
                return new List<int>(itemvalue as int[]);
            if (itemvalue is List<int>)
                return itemvalue as List<int>;

            var desValue = itemvalue.ToString()
                .Split(new[] {',', ' ', '|', '\t'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
            return desValue;
        }
    }
}