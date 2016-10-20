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
            return View(await _context.GetVideosAsync(info,pageIndex: page));
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
            var videoItem = await _context.GetSingleVideoItem(info,id);
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
        /// Show videos by the owner's channel that match the string/template selected
        /// </summary>
        /// <param name="model">Video Item containing required information</param>
        [HttpPost]
        public IActionResult DisplayGroups(VideoItem model)
        {
            return RedirectToAction("DisplayGroups",
                new RouteValueDictionary { { "channelId", model.ChannelId }, { "groupingSelected", model.GroupingSelected } });

        }

        /// <summary>
        /// Display videos by specified channel that match string or template.
        /// </summary>
        /// <param name="channelId">Id of Channel</param>
        /// <param name="groupingSelected">string or template</param>
        /// <param name="page">Page of results</param>
        [Route("Videos/DisplayGroups/{channelId}/{groupingSelected}")]
        public async Task<IActionResult> DisplayGroups(string channelId, string groupingSelected, int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            return View("Index", await _context.GetVideosByGroupingAsync(info, channelId, groupingSelected, pageIndex: page));
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
    }


}
