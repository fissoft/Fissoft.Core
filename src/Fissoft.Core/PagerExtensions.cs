using System;
using System.Collections.Generic;
using System.Linq;
using Fissoft.EntitySearch;

namespace Fissoft
{
    public static class PagerExtensions
    {
        const int DefaultPageSize = 20;
        public static PagedList<T> Pager<T>(this  IQueryable<T> source, int currentPage, int pageSize)
        {
            if (currentPage < 1)
                throw new ArgumentOutOfRangeException("currentPage",
                                                      @"当前参数currentPage必须>=1.");
            return new PagedList<T>(source, currentPage, pageSize);
        }

        public static PagedList<T> Pager<T>(this  IQueryable<T> source, int currentPage)
        {
            return source.Pager(currentPage, DefaultPageSize);
        }
        public static PagedList<T> Pager<T>(this  IEnumerable<T> source, int currentPage, int pageSize)
        {
            if (currentPage < 1)
                throw new ArgumentOutOfRangeException("currentPage",
                                                      @"当前参数currentPage必须>=1.");
            return new PagedList<T>(source, currentPage, pageSize);
        }
        public static PagedList<T> Pager<T>(this  IEnumerable<T> source, int currentPage)
        {
            return source.Pager(currentPage, DefaultPageSize);
        }
        /// <summary>
        /// 对数据进行分页操作，并按参数进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static PagedList<T> Pager<T>(this IQueryable<T> query, SearchModel m)
        {
            if (string.IsNullOrEmpty(m.SortName))
                throw new Exception("请设置SearchModel的DefaultSortOption方法，在调用Pager前对调用OrderBy方法");
            return query.OrderBy(m.SortName, m.SortOrder).Pager(m.Page, m.PageSize);
        }
    }
}
