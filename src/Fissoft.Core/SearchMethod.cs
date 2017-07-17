//-----------------------------------------------------------------------
// <copyright file="SearchMethod" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-09</addtime>
//-----------------------------------------------------------------------
using Fissoft.Framework.Systems.Attributes;
using System.ComponentModel;
namespace Fissoft.Framework.Systems
{
    /// <summary>
    /// Html表单元素的检索方式
    /// </summary>
    public enum SearchMethod
    {
        /// <summary>
        /// 等于
        /// </summary>
        //[SearchCode("等于")]
        [GlobalCode("=", OnlyAttribute = true)]
        Equal = 0,

        /// <summary>
        /// 小于
        /// </summary>
        //[SearchCode("小于")]
        [GlobalCode("<", OnlyAttribute = true)]
        LessThan = 1,

        /// <summary>
        /// 大于
        /// </summary>
        [GlobalCode(">", OnlyAttribute = true)]
        //[SearchCode("大于")]
        GreaterThan = 2,

        /// <summary>
        /// 小于等于
        /// </summary>
        //[SearchCode("小于等于")]
        [GlobalCode("<=", OnlyAttribute = true)]
        LessThanOrEqual = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        //[SearchCode("大于等于")]
        [GlobalCode(">=", OnlyAttribute = true)]
        GreaterThanOrEqual = 4,

        /// <summary>
        /// Like
        /// </summary>
        //[SearchCode("包含")]
        [GlobalCode("like", OnlyAttribute = true)]
        Like = 6,

        /// <summary>
        /// In
        /// </summary>
        //[SearchCode("包含于")]
        [GlobalCode("in", OnlyAttribute = true)]
        In = 7,

        /// <summary>
        /// 输入一个时间获取当前天的时间块操作, ToSql未实现，仅实现了IQueryable
        /// </summary>
        //[SearchCode("之间")]
        [GlobalCode("between", OnlyAttribute = true)]
        DateBlock = 8,

        //[SearchCode("不等于")]
        [GlobalCode("<>", OnlyAttribute = true)]
        NotEqual = 9,


        //[SearchCode("开始于")]
        [GlobalCode("like", OnlyAttribute = true)]
        StartsWith = 10,

        //[SearchCode("结束于")]
        [GlobalCode("like", OnlyAttribute = true)]
        EndsWith = 11,

        /// <summary>
        /// 处理Like的问题
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        //[SearchCode("包含")]
        [GlobalCode("like", OnlyAttribute = true)]
        Contains = 12,

        /// <summary>
        /// 处理In的问题,只支持 字段串列表
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        StdIn = 13,

        /// <summary>
        /// 处理Datetime小于+23h59m59s999f的问题
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        DateTimeLessThanOrEqual = 14,
        /// <summary>
        /// Not In
        /// </summary>
        [GlobalCode("not in", OnlyAttribute = true)]
        NotIn = 15,
        /// <summary>
        /// 只支持 字段串列表
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        StdNotIn = 16,

        //[SearchCode("已赋值")]
        IsNull = 18,

        //[SearchCode("值为空")]
        IsNullOrEmpty = 20,

        //[SearchCode("不包含")]
        [GlobalCode("not like", OnlyAttribute = true)]
        NotContains = 22,
    }
}