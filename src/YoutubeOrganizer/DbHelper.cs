using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;
using Sakura.AspNetCore;

namespace YoutubeOrganizer
{
    /// <summary>
    /// Static class containing methods for interacting with databases.
    /// </summary>
    public static class DbHelper
    {
        /// <summary>
        /// Return list of videos sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="pageIndex">The number of the page to be returned</param>
        /// <returns></returns>
        public static async Task<PagedVideoList> GetVideosAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, int pageSize = 25, int pageIndex = 1)
        {
            var userVideoResults = await context.UserVideo.Where(x => x.UserID.Equals(userInfo.ProviderKey)).ToListAsync();

            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                orderby video.PublishDate descending
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelId = channel.Id,
                                                    ChannelTitle = channel.ChannelTitle,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL,
                                                    Watched = userVideoResults.FirstOrDefault(x => x.VideoId.Equals(video.Id)).Watched
                                                };
            return await FromLiveItemsAsync(liveResults, pageSize, pageIndex);
        }

        /// <summary>
        /// Return MyPagedList of videos watched by user.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="pageIndex">1-Based index of page</param>
        public static async Task<PagedVideoList> GetVideosWatchedAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, int pageSize = 25, int pageIndex = 1)
        {
            List<UserVideo> userVideoResults = await context.UserVideo.Where(x => x.UserID.Equals(userInfo.ProviderKey) && x.Watched).ToListAsync();

            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                orderby video.PublishDate descending
                                                where userVideoResults.Select(x=>x.VideoId).Contains(video.Id)
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelId = channel.Id,
                                                    ChannelTitle = channel.ChannelTitle,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL,
                                                    Watched = true
                                                };
            return await FromLiveItemsAsync(liveResults, pageSize, pageIndex);
        }

        /// <summary>
        /// Return list of videos by channel that contain a string or template, sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="channelId">ID of channel</param>
        /// <param name="groupingSelected">String or template which videos should contain or match</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="pageIndex">The number of the page to be returned</param>
        public static async Task<PagedVideoList> GetVideosByGroupingAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, string channelId, string groupingSelected, int pageSize = 25, int pageIndex = 1)
        {
            var userVideoResults = await context.UserVideo.Where(x => x.UserID.Equals(userInfo.ProviderKey)).ToListAsync();

            string escapedGrouping = Regex.Escape(groupingSelected);
            var rgx = groupingSelected.Contains("#num") ? new Regex(escapedGrouping.Replace("#num", @"#\d+")) : new Regex(escapedGrouping);
            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                where rgx.IsMatch(video.Title) && video.ChannelId.Equals(channelId)
                                                orderby video.PublishDate descending
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelId = channel.Id,
                                                    ChannelTitle = channel.ChannelTitle,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL,
                                                    Watched = userVideoResults.FirstOrDefault(x => x.VideoId.Equals(video.Id)).Watched
                                                };
            var pagedList = await FromLiveItemsAsync(liveResults, pageSize, pageIndex);
            pagedList.ChannelId = channelId;
            pagedList.Grouping = groupingSelected;
            return pagedList;
        }

        /// <summary>
        /// Return list of videos by channel, sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="channelId">ID of channel</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="pageIndex">The number of the page to be returned</param>
        public static async Task<PagedVideoList> GetVideosByChannelAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, string channelId, int pageSize = 25, int pageIndex = 1)
        {
            var userVideoResults = await context.UserVideo.Where(x => x.UserID.Equals(userInfo.ProviderKey)).ToListAsync();
            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                where video.ChannelId.Equals(channelId)
                                                orderby video.PublishDate descending
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelId = channel.Id,
                                                    ChannelTitle = channel.ChannelTitle,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL,
                                                    Watched = userVideoResults.FirstOrDefault(x => x.VideoId.Equals(video.Id)).Watched
                                                };
            var pagedList = await FromLiveItemsAsync(liveResults, pageSize, pageIndex);
            pagedList.ChannelId = channelId;
            return pagedList;
        }

        /// <summary>
        /// Get a single video and its relation to user.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="videoId">ID of video</param>
        /// <returns></returns>
        public static async Task<VideoItem> GetSingleVideoItem(this ApplicationDbContext context,ShortLoginInfo userInfo, string videoId)
        {
            var userVideoResults = await context.UserVideo.Where(x => x.UserID.Equals(userInfo.ProviderKey)).ToListAsync();

            IQueryable<VideoItem> liveResults = from video in context.VideoItem
                                                join channel in context.ChannelItem on video.ChannelId equals channel.Id
                                                where video.Id.Equals(videoId)
                                                orderby video.PublishDate descending
                                                select new VideoItem
                                                {
                                                    Id = video.Id,
                                                    ChannelId = channel.Id,
                                                    ChannelTitle = channel.ChannelTitle,
                                                    Title = video.Title,
                                                    Duration = video.Duration,
                                                    PublishDate = video.PublishDate,
                                                    ThumbnailUrl = video.ThumbnailUrl,
                                                    VideoURL = video.VideoURL,
                                                    Watched = userVideoResults.FirstOrDefault(x => x.VideoId.Equals(video.Id)).Watched
                                                };
            var result = await liveResults.FirstAsync();
            return result;
        }

        /// <summary>
        /// Get login info for current user from database.
        /// </summary>
        /// <param name="userManager">Database holding user information</param>
        /// <param name="httpContext">Current context containing current user info</param>
        /// <returns></returns>
        public static async Task<ShortLoginInfo> GetCurrentLoginInfoAsync(this UserManager<ApplicationUser> userManager, HttpContext httpContext)
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            return user?.GetLoginInfo();
        }

        /// <summary>
        /// Make a PagedVideoList from IQueryable&lt;VideoItem&gt;.
        /// </summary>
        /// <param name="liveItems">Video items</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <param name="pageIndex">1-Based index of page</param>
        /// <returns></returns>
        public static async Task<PagedVideoList> FromLiveItemsAsync(IQueryable<VideoItem> liveItems, int pageSize = 25, int pageIndex = 1)
        {
            var list = await liveItems.ToListAsync();
            PagedList<IQueryable<VideoItem>, VideoItem> pagedList = await liveItems.ToPagedListAsync(pageSize, pageIndex);
            var preparedList = (PagedVideoList)pagedList;
            preparedList.TotalWatchedCount = list.Count(video => video.Watched);
            return preparedList;
        }
    }
}
