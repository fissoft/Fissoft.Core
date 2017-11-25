//-----------------------------------------------------------------------
// <copyright file="SearchModel" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-09</addtime>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fissoft.EntitySearch
{
    /// <summary>
    ///     �û��Զ��ռ�����������Model
    /// </summary>
    public class SearchModel
    {
        #region Ctor

        public SearchModel()
        {
            //PrivateInit(10);
        }

        #endregion

        /// <summary>
        /// �ж�ָ�� Member �Ƿ��в�ѯ����
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        [Obsolete]
        public bool HashSearchFilter(string memberName)
        {
            return Items.Where(m => m.Field.Equals(memberName, StringComparison.CurrentCultureIgnoreCase))
                       .FirstOrDefault() != null;
        }


        #region MatchModel

        /// <summary>
        ///     �ж϶����Ƿ���������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsMatch(object model)
        {
            var modelType = model.GetType();
            foreach (var item in Items)
            {
                var info = modelType.GetTypeInfo().GetProperty(item.Field);
                if (info != null && !item.IsMatch(info.GetValue(model, null)))
                    return false;
            }
            return true;
        }

        #endregion

        #region �������� ���ڷ�ҳ�������Լ���ѯ

        /// <summary>
        ///     ҳ��
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        ///     ÿҳ����
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        ///     Ҫ������ֶ�
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        ///     ����˳�� lf
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        ///     ��ѯ����
        /// </summary>
        public List<SearchItem> Items { get; set; } = new List<SearchItem>();

        public SearchModel AddSearchItem(SearchItem item)
        {
            Items.Add(item);
            return this;
        }

        public SearchModel AddSearchItem(string field, object val)
        {
            var method = SearchMethod.Equal;
            return AddSearchItem(new SearchItem(field, method, val));
        }

        public SearchModel AddSearchItem(string field, SearchMethod method, object val)
        {
            return AddSearchItem(new SearchItem(field, method, val));
        }

        public SearchModel AddSearchItem(string field, SearchMethod method, object val, string id)
        {
            return AddSearchItem(new SearchItem(field, method, val, id));
        }

        #endregion
    }
}