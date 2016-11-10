using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// ViewModel for universal pagination.
    /// </summary>
    public class PaginationViewModel
    {
        public readonly string ActionName;
        public readonly string ControllerName;
        public readonly int CurrentPage;
        public readonly int PageCount;
        public readonly bool IsFirstPage;
        public readonly bool IsLastPage;
        public readonly RouteValueDictionary RouteValues;

        /// <summary>
        /// Do not duplicate pageIndex in routeValues.
        /// </summary>
        public PaginationViewModel(string actionName, string controllerName, int currentPage, int pageCount, RouteValueDictionary routeValues = null)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            CurrentPage = currentPage;
            PageCount = pageCount;
            IsFirstPage = currentPage == 1;
            IsLastPage = currentPage == pageCount;
            if (routeValues == null)
            {
                RouteValues = new RouteValueDictionary {{"page", currentPage}};
            }
            else
            {
                routeValues.Add( "page", currentPage);
                RouteValues = routeValues;
            }
        }

        public PaginationViewModel()
        {
            
        }
    }
}
