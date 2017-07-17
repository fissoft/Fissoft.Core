//-----------------------------------------------------------------------
// <copyright file="SearchModel" company="Fissoft.com">
//     Copyright (c) Fissoft.com . All rights reserved.
// </copyright>
// <author>Zou Jian</author>
// <addtime>2010-09</addtime>
//-----------------------------------------------------------------------

namespace Fissoft.Framework.Systems
{
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System;
     
    using System.Reflection;


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

        /// <summary>
        /// 如果有查询则生成需要的标准SQL
        /// </summary>
        /// <returns></returns>
        public string ToSearchString()
        {
            return ToSearchStringAbstract(GetSearchString);
        }
        /// <summary>
        /// 如果有查询则生成需要的标准SQL,不带参数变量
        /// Add by guoqi.zhao
        /// </summary>
        /// <returns></returns>
        public string ToNoParamsSearchString()
        {
            return ToSearchStringAbstract(GetNoParamsSearchString);
        }
        public List<object> ParamList { get; set; }
        public string ToSearchStringAbstract(Action<string, IEnumerable<SearchItem>, StringBuilder> func)
        {
            ParamList.Clear();
            var sb = new StringBuilder();
            var andItems = Items.Where(c => string.IsNullOrEmpty(c.OrGroup)).ToList();
            func("AND", andItems, sb);
            var orItems = Items.Where(c => !string.IsNullOrEmpty(c.OrGroup)).ToList();
            //var nowIndex = andItems.Count(c => c.Method != SearchMethod.Like && c.Method != SearchMethod.In && c.Method != SearchMethod.NotIn);
            foreach (var searchItem in orItems.GroupBy(c => c.OrGroup))
            {
                sb.Append(" (");
                func("OR", searchItem, sb);
                sb.Remove(sb.Length - 2, 2);
                sb.Append(") AND");
                //nowIndex += searchItem.Count();
            }

            return sb.ToString().TrimEnd("AND".ToCharArray());
        }

        private void GetSearchString(string opr, IEnumerable<SearchItem> andItems, StringBuilder sb)//, int index
        {
            var list = andItems.ToList();
            //int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (!item.Value.ToString().Contains("%"))
                {
                    if (item.Method == SearchMethod.Like || item.Method == SearchMethod.Contains || item.Method==SearchMethod.NotContains)
                        item.Value = string.Format("%{0}%", item.Value);
                    else if (item.Method == SearchMethod.StartsWith)
                        item.Value = string.Format("{0}%", item.Value);
                    else if (item.Method == SearchMethod.EndsWith)
                        item.Value = string.Format("%{0}", item.Value);
                }
                if (item.Method == SearchMethod.In || item.Method == SearchMethod.NotIn)
                {
                    //当为In时要有一个()
                    sb.AppendFormat(" {0} {1} ({2}) " + opr, item.Field, item.Method.GetGlobalCode(), GetInValue(item.Value));
                }
                else if (item.Method == SearchMethod.IsNull)
                {
                    string rightConValue = item.Value == null ? string.Empty : item.Value.ToString();
                    if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                        sb.AppendFormat(" {0} IS NULL " + opr, item.Field);
                    else
                        sb.AppendFormat(" {0} IS NOT NULL " + opr, item.Field);
                }
                else if (item.Method == SearchMethod.IsNullOrEmpty)
                {
                    string rightConValue = item.Value == null ? string.Empty : item.Value.ToString();
                    if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                        sb.AppendFormat(" ({0} IS NULL OR N''={0}) " + opr, item.Field);
                    else
                        sb.AppendFormat(" {0} IS NOT NULL AND N''<>{0} " + opr, item.Field);
                }
                else
                {
                    sb.AppendFormat(" {0} {1} @p{2} " + opr, item.Field, item.Method.GetGlobalCode(), ParamList.Count);
                    // index++;
                    ParamList.Add(item.Value);
                }               
            }
        }
        private static object GetInValue(object itemvalue)
        {
            if (itemvalue is int[])
                return string.Join(",", (itemvalue as int[]).Select(x => x.ToString()));
            else if (itemvalue is List<int>)
                return string.Join(",", (itemvalue as List<int>).Select(x => x.ToString()));

            if (itemvalue is float[])
                return string.Join(",", (itemvalue as float[]).Select(x => x.ToString()));
            else if (itemvalue is List<float>)
                return string.Join(",", (itemvalue as List<float>).Select(x => x.ToString()));

            else if (itemvalue is long[])
                return string.Join(",", (itemvalue as long[]).Select(x => x.ToString()));
            else if (itemvalue is List<long>)
                return string.Join(",", (itemvalue as List<long>).Select(x => x.ToString()));

            if (itemvalue is double[])
                return string.Join(",", (itemvalue as double[]).Select(x => x.ToString()));
            else if (itemvalue is List<double>)
                return string.Join(",", (itemvalue as List<double>).Select(x => x.ToString()));

            if (itemvalue is string)
            {
                string tempStr = itemvalue.ToString();
                List<string> items = new List<string>(tempStr.Split(new char[]{',','\n','\t'},  StringSplitOptions.RemoveEmptyEntries));
                return items.ToJoinString("','").ToLikingString('\'');
            }
            return itemvalue;
        }


        private void GetNoParamsSearchString(string optr, IEnumerable<SearchItem> andItems, StringBuilder sb)
        {
            var list = andItems.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];// hujingang 2010-11-29 17:22:40
                if (!item.Value.ToString().Contains("%"))
                {
                    if (item.Method == SearchMethod.Like || item.Method == SearchMethod.Contains || item.Method == SearchMethod.NotContains)
                        item.Value = string.Format("%{0}%", item.Value);
                    else if (item.Method == SearchMethod.StartsWith)
                        item.Value = string.Format("{0}%", item.Value);
                    else if (item.Method == SearchMethod.EndsWith)
                        item.Value = string.Format("%{0}", item.Value);
                }
                if (item.Method == SearchMethod.In || item.Method == SearchMethod.NotIn)
                {
                    sb.AppendFormat(" {0} {1} ({2}) " + optr, item.Field, item.Method.GetGlobalCode(), GetInValue(item.Value));
                }
                else if (item.Method == SearchMethod.IsNull)
                {
                    string rightConValue = item.Value == null ? string.Empty : item.Value.ToString();
                    if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                        sb.AppendFormat(" {0} IS NULL " + optr, item.Field);
                    else
                        sb.AppendFormat(" {0} IS NOT NULL " + optr, item.Field);
                }
                else if (item.Method == SearchMethod.IsNullOrEmpty)
                {
                    string rightConValue = item.Value == null ? string.Empty : item.Value.ToString();
                    if (rightConValue == "True" || rightConValue == "true" || rightConValue == "1")
                        sb.AppendFormat(" ({0} IS NULL OR N''={0}) " + optr, item.Field);
                    else
                        sb.AppendFormat(" {0} IS NOT NULL AND N''<>{0} " + optr, item.Field);
                }
                else
                {
                    sb.AppendFormat(" {0} {1} '{2}' " + optr, item.Field, item.Method.GetGlobalCode(), item.Value);
                }
            }
        }

        /// <summary>
        /// 根据查询条件生成带Where的SQL
        /// added by Li Bo 2010-09-15
        /// </summary>
        /// <returns></returns>
        public string ToWhereSearchString(bool hasParam = true)
        {
            string sqlStr;
            if (hasParam)
                sqlStr = ToSearchString();
            else
                sqlStr = ToNoParamsSearchString();
            return string.IsNullOrEmpty(sqlStr) ? "" : " WHERE " +  sqlStr ;
        }

        

        /// <summary>
        /// 生成排序的SQL
        /// added by Li Bo 2010-09-15
        /// edit by Tian Jing 2010-10-13
        /// </summary>
        /// <returns></returns>
        public string ToOrderString(string extentTbName = "")
        {
            return ToOrderString(SortName, SortOrder, extentTbName);
        }
        /// <summary>
        /// 生成排序
        /// added by Tian Jing 2010-10-13
        /// edit by Li Bo 2010-12-22
        /// </summary>
        /// <returns></returns>
        public static string ToOrderString(string sortName, string sortOrder,string extentTbName="")
        {
            if (string.IsNullOrEmpty(sortName))
                return "";
            string extentTbNameStr ="";
            if (!string.IsNullOrEmpty(extentTbName))
            {
                extentTbNameStr = string.Format("[{0}].", extentTbName);
                int index = sortName.IndexOf('.');
                if (index > -1)
                    sortName = sortName.Substring(index + 1);
            }
            return string.Format(" ORDER BY {2}{0} {1} ", sortName, sortOrder, extentTbNameStr);
        }

        /// <summary>
        /// [zj]
        /// 使用StoreCommand等需要传入参数进行查询的方法时，对应的参数列表
        /// </summary>
        /// <returns></returns>
        public object[] GetObjectParmaters()
        {
            return ParamList.ToArray();
            // return Items.Where(x => x.Method != SearchMethod.In).Select(c => c.Value).ToArray();
        }

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
                PropertyInfo info = modelType.GetProperty(item.Field);
                if (info!=null && !item.IsMatch(info.GetValue(model, null)))
                    return false;
            }
            return true;
        }



        public string ToUrlParamsString()
        {
            StringBuilder  builder = new StringBuilder();
            foreach (SearchItem item in this.Items)
                builder.AppendFormat("{0}&", item.ToUrlParamsString());  
            if (builder.Length > 0)
                builder = builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
        #endregion

    }
}