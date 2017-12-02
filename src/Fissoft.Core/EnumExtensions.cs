//-----------------------------------------------------------------------
// <copyright file="EnumExtensions" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-09</addtime>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Fissoft.Attributes;

namespace Fissoft
{
    /// <summary>
    ///     枚举的扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        #region 获取Enum的GlobalCodeAttribute

        public static GlobalCodeAttribute GetGlobalCodeAttribute(Enum e)
        {
            Contract.Requires(e != null);
            var type = e.GetType();
            var memInfo = type.GetTypeInfo().GetMember(e.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(GlobalCodeAttribute), false).ToArray();
                if (attrs.Any())
                    return (GlobalCodeAttribute) attrs.FirstOrDefault();
            }
            return null;
        }

        #endregion


        public static List<int> ToIntList<TEnum>(this List<TEnum> list)
        {
            var intList = new List<int>();
            foreach (var item in list)
                intList.Add(Convert.ToInt32(item));
            return intList;
        }

        /// <summary>
        ///     得到emum的所有key列表
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IList<string> GetNames(this Enum e)
        {
            IList<string> enumNames = new List<string>();
            foreach (var fi in e.GetType().GetTypeInfo().GetFields(BindingFlags.Static | BindingFlags.Public))
                enumNames.Add(fi.Name);
            return enumNames;
        }

        /// <summary>
        ///     得到enum所有的value列表
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Array GetValues(this Enum e)
        {
            var enumValues = new List<int>();
            foreach (var fi in e.GetType().GetTypeInfo().GetFields(BindingFlags.Static | BindingFlags.Public))
                enumValues.Add((int) Enum.Parse(e.GetType(), fi.Name, false));
            return enumValues.ToArray();
        }


        public static string ToIntString(this Enum value)
        {
            return ((int) (object) value).ToString();
        }

        #region 获取属性

        /// <summary>
        ///     获取Enum的GlobalCode标记中的描述信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetGlobalCode(this Enum e)
        {
            var attr = GetGlobalCodeAttribute(e);
            return attr != null ? attr.Description : e.ToString();
        }

        /// <summary>
        ///     获取Enum的GlobalCode标记中的颜色信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns>返回表示颜色的字符串</returns>
        public static string GetDisplayColor(this Enum e)
        {
            var attr = GetGlobalCodeAttribute(e);

            if (attr != null)
                return attr.Color;

            return string.Empty;
        }

        /// <summary>
        ///     返回扩展名称，tj需要
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetExtName(this Enum e)
        {
            var attr = GetGlobalCodeAttribute(e);
            if (attr != null)
                return attr.ExtName;
            return string.Empty;
        }

        #endregion
    }
}