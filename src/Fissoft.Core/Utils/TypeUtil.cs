//-----------------------------------------------------------------------
// <copyright file="TypeUtil" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-11</addtime>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Reflection;

namespace Fissoft.Utils
{
    /// <summary>
    ///     Type类的处理工具类
    /// </summary>
    public class TypeUtil
    {
        /// <summary>
        ///     获取可空类型的实际类型
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static Type GetUnNullableType(Type conversionType)
        {
            if (conversionType.GetTypeInfo().IsGenericType &&
                conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //如果是泛型方法，且泛型类型为Nullable<>则视为可空类型
                //并使用NullableConverter转换器进行转换
                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return conversionType;
        }
    }
}