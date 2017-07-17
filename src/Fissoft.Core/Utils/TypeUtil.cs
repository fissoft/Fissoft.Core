//-----------------------------------------------------------------------
// <copyright file="TypeUtil" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-11</addtime>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace Fissoft.Utils
{
    /// <summary>
    /// Type��Ĵ���������
    /// </summary>
    public class TypeUtil
    {
        /// <summary>
        /// ��ȡ�ɿ����͵�ʵ������
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static Type GetUnNullableType(Type conversionType)
        {
            if (conversionType.GetTypeInfo().IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //����Ƿ��ͷ������ҷ�������ΪNullable<>����Ϊ�ɿ�����
                //��ʹ��NullableConverterת��������ת��
                var nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return conversionType;
        }
    }
}