using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;
using Sakura.AspNetCore;
using YoutubeOrganizer.Controllers;

namespace YoutubeOrganizer
{
    public static class DbHelper
    {
        /// <summary>
        /// Return list of videos sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <returns></returns>
        public static async Task<IEnumerable<VideoItem>> GetVideosAsync(this ApplicationDbContext context)
        {
            IQueryable<VideoItem> results = from video in context.VideoItem
                                            join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                            orderby video.PublishDate descending
                                            select new VideoItem
                                            {
                                                ChannelTitle = channel.Title,
                                                Title = video.Title,
                                                Duration = video.Duration,
                                                PublishDate = video.PublishDate,
                                                ThumbnailUrl = video.ThumbnailUrl,
                                                VideoURL = video.VideoURL
                                            };
            return await results.ToListAsync();
        }

        /// <summary>
        /// Return list of videos sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="pageNumber">The number of the page to be returned</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <returns></returns>
        public static async Task<PagedList<IQueryable<VideoItem>, VideoItem>> GetPagedVideosAsync(this ApplicationDbContext context, int pageNumber = 1, int pageSize = 25)
        {
            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                            join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                            orderby video.PublishDate descending
                                            select new VideoItem
                                            {
                                                Id = video.Id,
                                                ChannelTitle = channel.Title,
                                                Title = video.Title,
                                                Duration = video.Duration,
                                                PublishDate = video.PublishDate,
                                                ThumbnailUrl = video.ThumbnailUrl,
                                                VideoURL = video.VideoURL
                                            };
            return await liveResults.ToPagedListAsync(pageSize, pageNumber);
        }


        public static async Task<MyPagedList<VideoItem>> GetMyPagedVideosAsync(this ApplicationDbContext context, int pageNumber = 1, int pageSize = 25)
        {
            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                orderby video.PublishDate descending
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelTitle = channel.Title,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL
                                                };
            var pagedList = await liveResults.ToPagedListAsync(pageSize, pageNumber);
            return (MyPagedList<VideoItem>)pagedList;
        }

        public static async Task<ShortLoginInfo> GetCurrentLoginInfoAsync(this UserManager<ApplicationUser> userManager, HttpContext httpContext)
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            return user?.GetLoginInfo();
        }
    }
}
