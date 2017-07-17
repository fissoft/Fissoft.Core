using System;
using System.Collections.Generic;
using System.Linq;

namespace Fissoft.Framework.Systems
{
    using System.ComponentModel;

    /// <summary>
    /// 可以对IQueryable或IEnumerable进行分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IEnumerable<T> content, int currentPage, int pageSize, int totalCount)
            : this(totalCount, currentPage, pageSize)
        {
            AddRange(content);
        }
        public PagedList(IQueryable<T> source, int currentPage, int pageSize)
            : this(source.Count(), currentPage, pageSize)
        {
            AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList());
        }

        public PagedList(IEnumerable<T> source, int currentPage, int pageSize)
            : this(source.Count(), currentPage, pageSize)
        {
            AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList());
        }

        protected PagedList(int count, int currentPage, int pageSize)
        {
            TotalCount = count;
            PageSize = Math.Max(pageSize, 1);
            CurrentPage = Math.Min(Math.Max(currentPage, 1), TotalPages);
        }

        /// <summary>
        /// 当前页号
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 是否存在前一页
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;


        /// <summary>
        /// 是否存在后一页
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// 每页数据量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 扩展文本
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ExtString { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => Math.Max((TotalCount + PageSize - 1) / PageSize, 1);
    }
}