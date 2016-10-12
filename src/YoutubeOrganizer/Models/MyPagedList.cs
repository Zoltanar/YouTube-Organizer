using System.Collections.Generic;
using System.Linq;
using Sakura.AspNetCore;

namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// Custom PagedList to get around ambiguous inheritance issues in Sakura.AspNetCore.PagedList.
    /// </summary>
    /// <typeparam name="TSource">Type of objects in list</typeparam>
    public class MyPagedList<TSource> : List<TSource>
    {
        /// <summary>
        /// Number of total items, across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Number of total pages.
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Index of current page.
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Size of pages.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Is this page the first?
        /// </summary>
        public bool IsFirstPage { get; set; }

        /// <summary>
        /// Is this page the last?
        /// </summary>
        public bool IsLastPage { get; set; }

        // ReSharper disable once EmptyConstructor
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public MyPagedList()
        { }

        /// <summary>
        /// Explicit cast from PagedList.
        /// </summary>
        /// <param name="pagedList">PagedList to cast from.</param>
        public static explicit operator MyPagedList<TSource>(PagedList<IQueryable<TSource>, TSource> pagedList)
        {
            var list = new MyPagedList<TSource>
            {
                TotalPage = pagedList.TotalPage,
                TotalCount = pagedList.TotalCount,
                PageIndex = pagedList.PageIndex,
                PageSize = pagedList.PageSize,
                IsFirstPage = pagedList.IsFirstPage(),
                IsLastPage = pagedList.IsLastPage()
            };
            list.AddRange(pagedList);
            return list;
        }
    }
}
