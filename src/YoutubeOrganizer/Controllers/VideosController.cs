using System.Collections.Generic;
using System.Linq;
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
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VideosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Videos
        public async Task<IActionResult> Index(int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            return View(await _context.GetMyPagedVideosAsync(info, page));
        }
        [HttpPost]
        public async Task<IActionResult> Index(MyPagedList<VideoItem> videos, int page = 1)
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
            videoItem.Groupings = new SelectList(parts);
            return View(videoItem);
        }

        [HttpPost]
        public IActionResult DisplayGroups(VideoItem model)
        {
            return RedirectToAction("DisplayGroups",
                new RouteValueDictionary { { "channelId", model.ChannelId }, { "groupingSelected", model.GroupingSelected } });

        }

        [Route("Videos/DisplayGroups/{channelId}/{groupingSelected}")]
        public async Task<IActionResult> DisplayGroups(string channelId, string groupingSelected, int page = 1)
        {
            var info = await _userManager.GetCurrentLoginInfoAsync(HttpContext);
            return View("Index", await _context.GetMyPagedVideosByGroupingAsync(info, channelId, groupingSelected, page));

        }
    }


}
