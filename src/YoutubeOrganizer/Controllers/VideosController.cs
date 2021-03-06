using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;

namespace YoutubeOrganizer.Controllers
{
    /// <summary>
    /// Controller for views about videos.
    /// </summary>
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Object Database</param>
        /// <param name="userManager">User Database</param>
        public VideosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Videos
        /// <summary>
        /// Landing page for Videos, displays all videos, paginated.
        /// </summary>
        /// <param name="page">Index of page to be displayed</param>
        public async Task<IActionResult> Index(int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            return View(await _context.GetVideosAsync(info, pageIndex: page));
        }

        /// <summary>
        /// Save changes for watched status of videos.
        /// </summary>
        /// <param name="videos">List of videos to be changed</param>
        /// <param name="page">Index of page to return to</param>
        [HttpPost]
        public async Task<IActionResult> Index(PagedVideoList videos, int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            List<string> currentUserVideos = await _context.UserVideo.Where(entry => entry.UserID.Equals(info.ProviderKey)).Select(entry => entry.VideoId).ToListAsync();
            foreach (var video in videos)
            {
                if (currentUserVideos.Contains(video.Id))
                {
                    _context.UserVideo.Update(new UserVideo
                    {
                        UserID = info.ProviderKey,
                        VideoId = video.Id,
                        Watched = video.Watched
                    });
                }
                else
                {
                    _context.UserVideo.Add(new UserVideo
                    {
                        UserID = info.ProviderKey,
                        VideoId = video.Id,
                        Watched = video.Watched
                    });
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Videos", new RouteValueDictionary { { "page", page } });
        }

        // GET: Videos/Details/5
        /// <summary>
        /// Display details of a single video.
        /// </summary>
        /// <param name="id">Id of video</param>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            var videoItem = await _context.GetSingleVideoItem(info, id);
            if (videoItem == null)
            {
                return NotFound();
            }
            //TODO move split parts to its own place more splits ||, ? (u2013), allow with numbers
            var parts = videoItem.Title.Split('-');
            var rgx = new Regex(@"#\d+");
            var selectList = parts.Select(part => rgx.IsMatch(part) ? rgx.Replace(part, "#num") : part).ToList();
            videoItem.Groupings = new SelectList(selectList);
            return View(videoItem);
        }

        /// <summary>
        /// Save video details.
        /// </summary>
        /// <param name="video">video</param>
        [HttpPost]
        public async Task<IActionResult> Details(VideoItem video)
        {
            if (video.Id == null)
            {
                return NotFound();
            }
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            var videoItem = await _context.GetSingleVideoItem(info, video.Id);
            if (videoItem == null)
            {
                return NotFound();
            }
            var existingVideo =
               await  _context.UserVideo.FirstOrDefaultAsync(x => x.UserID.Equals(info.ProviderKey) && x.VideoId.Equals(video.Id));
            if (existingVideo == null)
                _context.UserVideo.Add(new UserVideo
                {
                    UserID = info.ProviderKey,
                    VideoId = video.Id,
                    Watched = video.Watched
                });
            else
            {
                existingVideo.Watched = video.Watched;
                _context.UserVideo.Update(existingVideo);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details",new RouteValueDictionary { {"Id",video.Id} });
        }


        /// <summary>
        /// Display Watched Videos.
        /// </summary>
        /// <param name="page"></param>
        public async Task<IActionResult> WatchedVideos(int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            return View(await _context.GetVideosWatchedAsync(info, pageIndex: page));
        }

        /// <summary>
        /// Mark all videos of group as watched by the user.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IActionResult> MarkGroupAsWatched(string channelId, string grouping)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            if (info == null) return RedirectToAction("Index", "Home");
            await _context.MarkVideoGroupAsWatchedAsync(info, channelId, grouping);
            return RedirectToAction("Group", "Videos", new RouteValueDictionary { { "channelId", channelId }, { "grouping", grouping } });
        }

        /// <summary>
        /// Save group of videos to user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveUserGroup(string channelId, string grouping, string groupName)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            if (info == null) return RedirectToAction("Index", "Home");
            var userGroup = new UserGroup(info.ProviderKey, channelId, groupName, grouping);
            _context.UserGroup.Add(userGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction("Group", "Videos",new RouteValueDictionary { { "channelId", channelId }, { "grouping", grouping } });
        }

        /// <summary>
        /// Display videos that match grouping.
        /// </summary>
        /// <param name="grouping"></param>
        /// <param name="page">Page of results</param>
        /// <param name="channelId">Id of videos' owner channel</param>
        //[Route("Videos/Group/{channelId}/{groupingTemplate}/{page=1}")]
        public async Task<IActionResult> Group(string channelId, string grouping, int page = 1)
        {
            if(channelId == null || grouping == null) return RedirectToAction("Index", "Home");
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            if (info == null) return RedirectToAction("Index", "Home");
            return View(await
                        _context.GetVideosByGroupingAsync(info, channelId, grouping, pageIndex: page));
        }

        /// <summary>
        /// Return group of videos from video details page.
        /// </summary>
        /// <param name="video">Video that forwarded to this method.</param>
        [HttpPost]
        public IActionResult GroupByVideo(VideoItem video)
        {
            return RedirectToAction("Group",new RouteValueDictionary {{"channelId", video.ChannelId}, {"grouping", video.GroupingSelected}});
        }
    }


}
