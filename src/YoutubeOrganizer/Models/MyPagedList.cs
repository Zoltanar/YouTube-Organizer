using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sakura.AspNetCore;

namespace YoutubeOrganizer.Models
{
    public class MyPagedList<TSource> : List<TSource>
    {
        public int TotalCount { get; set; }

        public int TotalPage { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
        // ReSharper disable once EmptyConstructor
        public MyPagedList()
        { }
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
