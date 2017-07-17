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
    /// 用户自动收集搜索条件的Model
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

        #region 基础属性 用于分页、排序以及查询

        /// <summary>
        /// 页码 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 要排序的字段
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// 排序顺序 lf
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// 查询条件
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
        #region 生成SQL及参数的方法



        public List<object> ParamList { get; set; }
 

        


        #endregion
 

        #region MatchModel

        /// <summary>
        /// 判断对象是否满足条件
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