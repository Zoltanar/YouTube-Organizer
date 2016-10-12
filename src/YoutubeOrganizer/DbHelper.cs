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
        /// <param name="pageNumber">The number of the page to be returned</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <returns></returns>
        public static async Task<MyPagedList<VideoItem>> GetVideosAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, int pageNumber = 1, int pageSize = 25)
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
            var pagedList = await liveResults.ToPagedListAsync(pageSize, pageNumber);
            return (MyPagedList<VideoItem>)pagedList;
        }

        /// <summary>
        /// Return MyPagedList of videos watched by user.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="pageNumber">The number of the page to be returned</param>
        /// <param name="pageSize">The number of results per page</param>
        public static async Task<MyPagedList<VideoItem>> GetVideosWatchedAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, int pageNumber = 1, int pageSize = 25)
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
            PagedList<IQueryable<VideoItem>, VideoItem> pagedList = await liveResults.ToPagedListAsync(pageSize, pageNumber);
            return (MyPagedList<VideoItem>)pagedList;
        }

        /// <summary>
        /// Return list of videos by channel that contain a string or template, sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="channelId">ID of channel</param>
        /// <param name="groupingSelected">String or template which videos should contain or match</param>
        /// <param name="pageNumber">The number of the page to be returned</param>
        /// <param name="pageSize">The number of results per page</param>
        public static async Task<MyPagedList<VideoItem>> GetVideosByGroupingAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, string channelId, string groupingSelected, int pageNumber = 1, int pageSize = 25)
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
            PagedList<IQueryable<VideoItem>, VideoItem> pagedList = await liveResults.ToPagedListAsync(pageSize, pageNumber);
            return (MyPagedList<VideoItem>)pagedList;
        }

        /// <summary>
        /// Return list of videos by channel, sorted by PublishDate.
        /// </summary>
        /// <param name="context">Context containing database with videos</param>
        /// <param name="userInfo">Login Info of user</param>
        /// <param name="channelId">ID of channel</param>
        /// <param name="pageNumber">The number of the page to be returned</param>
        /// <param name="pageSize">The number of results per page</param>
        public static async Task<MyPagedList<VideoItem>> GetVideosByChannelAsync(this ApplicationDbContext context, ShortLoginInfo userInfo, string channelId, int pageNumber = 1, int pageSize = 25)
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
            var pagedList = await liveResults.ToPagedListAsync(pageSize, pageNumber);
            return (MyPagedList<VideoItem>)pagedList;
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
    }
}
