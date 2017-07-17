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
    /// Html��Ԫ�صļ�����ʽ
    /// </summary>
    public enum SearchMethod
    {
        /// <summary>
        /// ����
        /// </summary>
        //[SearchCode("����")]
        [GlobalCode("=", OnlyAttribute = true)]
        Equal = 0,

        /// <summary>
        /// С��
        /// </summary>
        //[SearchCode("С��")]
        [GlobalCode("<", OnlyAttribute = true)]
        LessThan = 1,

        /// <summary>
        /// ����
        /// </summary>
        [GlobalCode(">", OnlyAttribute = true)]
        //[SearchCode("����")]
        GreaterThan = 2,

        /// <summary>
        /// С�ڵ���
        /// </summary>
        //[SearchCode("С�ڵ���")]
        [GlobalCode("<=", OnlyAttribute = true)]
        LessThanOrEqual = 3,

        /// <summary>
        /// ���ڵ���
        /// </summary>
        //[SearchCode("���ڵ���")]
        [GlobalCode(">=", OnlyAttribute = true)]
        GreaterThanOrEqual = 4,

        /// <summary>
        /// Like
        /// </summary>
        //[SearchCode("����")]
        [GlobalCode("like", OnlyAttribute = true)]
        Like = 6,

        /// <summary>
        /// In
        /// </summary>
        //[SearchCode("������")]
        [GlobalCode("in", OnlyAttribute = true)]
        In = 7,

        /// <summary>
        /// ����һ��ʱ���ȡ��ǰ���ʱ������, ToSqlδʵ�֣���ʵ����IQueryable
        /// </summary>
        //[SearchCode("֮��")]
        [GlobalCode("between", OnlyAttribute = true)]
        DateBlock = 8,

        //[SearchCode("������")]
        [GlobalCode("<>", OnlyAttribute = true)]
        NotEqual = 9,


        //[SearchCode("��ʼ��")]
        [GlobalCode("like", OnlyAttribute = true)]
        StartsWith = 10,

        //[SearchCode("������")]
        [GlobalCode("like", OnlyAttribute = true)]
        EndsWith = 11,

        /// <summary>
        /// ����Like������
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        //[SearchCode("����")]
        [GlobalCode("like", OnlyAttribute = true)]
        Contains = 12,

        /// <summary>
        /// ����In������,ֻ֧�� �ֶδ��б�
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        StdIn = 13,

        /// <summary>
        /// ����DatetimeС��+23h59m59s999f������
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        DateTimeLessThanOrEqual = 14,
        /// <summary>
        /// Not In
        /// </summary>
        [GlobalCode("not in", OnlyAttribute = true)]
        NotIn = 15,
        /// <summary>
        /// ֻ֧�� �ֶδ��б�
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        StdNotIn = 16,

        //[SearchCode("�Ѹ�ֵ")]
        IsNull = 18,

        //[SearchCode("ֵΪ��")]
        IsNullOrEmpty = 20,

        //[SearchCode("������")]
        [GlobalCode("not like", OnlyAttribute = true)]
        NotContains = 22,
    }
}