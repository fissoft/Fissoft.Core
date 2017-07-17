using System.Collections;

namespace Fissoft.Framework.Systems
{
    /// <summary>
    /// �ɷ�ҳ�Ľӿ�
    /// </summary>
    public interface IPagedList : IEnumerable
    {
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        int TotalPages { get; }
        int CurrentPage { get; set; }
        int PageSize { get; set; }
        int TotalCount { get; set; }
        string ExtString { get; set; }
    }
}