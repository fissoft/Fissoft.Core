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
    ///     用户自动收集搜索条件的Model
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
        /// 判断指定 Member 是否有查询条件
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
        ///     判断对象是否满足条件
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

        #region 基础属性 用于分页、排序以及查询

        /// <summary>
        ///     页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        ///     每页条数
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        ///     要排序的字段
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        ///     排序顺序 lf
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        ///     查询条件
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