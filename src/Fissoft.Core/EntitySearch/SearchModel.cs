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
    /// �û��Զ��ռ�����������Model
    /// </summary>
    public class SearchModel 
    {
        #region Ctor

        public SearchModel()
        {
            PrivateInit(10);
        }

        #endregion

        public void PrivateInit(int pageSize)
        {
            if (Page == 0)
            {
                Page = 1;

                
            }
            if(Items==null)
            Items = new List<SearchItem>();
            if(ParamList==null)
            ParamList = new List<object>();
            PageSize = pageSize;
        }

        #region �������� ���ڷ�ҳ�������Լ���ѯ

        /// <summary>
        /// ҳ�� 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ÿҳ����
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Ҫ������ֶ�
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// ����˳�� lf
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// ��ѯ����
        /// </summary>
        public List<SearchItem> Items { get; set; }

        public SearchModel AddSearchItem(SearchItem item)
        {
            Items.Add(item);
            return this;
        }
        public SearchModel AddSearchItem(string field,  object val)
        {
            SearchMethod method = SearchMethod.Equal;
            return AddSearchItem(new SearchItem(field, method, val)); 
        }
        public SearchModel AddSearchItem(string field, SearchMethod method, object val)
        {
            return AddSearchItem( new SearchItem(field, method, val));
        }

        public SearchModel AddSearchItem(string field, SearchMethod method, object val,string id)
        {
            return AddSearchItem( new SearchItem(field, method, val, id));
        }
     
        #endregion

        public bool HashSearchFilter(string memberName)
        {
            return Items.Where(m => m.Field.Equals(memberName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault() != null;
        }
        #region ����SQL�������ķ���



        public List<object> ParamList { get; set; }
 

        


        #endregion
 

        #region MatchModel

        /// <summary>
        /// �ж϶����Ƿ���������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsMatch(object model)
        {
            Type modelType = model.GetType();
            foreach (SearchItem item in Items)
            {
                PropertyInfo info = modelType.GetTypeInfo().GetProperty(item.Field);
                if (info!=null && !item.IsMatch(info.GetValue(model, null)))
                    return false;
            }
            return true;
        }



        #endregion

    }
}