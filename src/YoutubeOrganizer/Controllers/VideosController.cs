using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Update;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;

namespace YoutubeOrganizer.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExternalLoginInfo _info;

        public VideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Videos
        public async Task<IActionResult> Index()
        {
            return View(await _context.VideoItem.ToListAsync());
        }

        // GET: Videos/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoItem = await _context.VideoItem.SingleOrDefaultAsync(m => m.Id.Equals(id));
            if (videoItem == null)
            {
                return NotFound();
            }
            //TODO move split parts to its own place more splits ||, ? (u2013), allow with numbers
            var parts = videoItem.Title.Split('-');
            videoItem.Groupings = new SelectList(parts);
            return View(videoItem);
        }

        // GET: Videos/Create
        public IActionResult Create()
        {
            return View();
        }

        [Route("Videos/DisplayGroups/{channelId}/{groupingSelected}")]
        public async Task<IActionResult> DisplayGroups(string channelId, string groupingSelected)
        {
            if (User != null)
            {
                _info = GlobalVariables.UserLoginInfo[User.Identity.Name];
            }
            IQueryable<VideoItem> results = from video in _context.VideoItem
                                            join channel in _context.ChannelItem on video.ChannelId equals channel.Id
                                            where video.Title.Contains(groupingSelected) &&
                                            video.ChannelId.Equals(channelId)
                                            select new VideoItem
                                            {
                                                ChannelTitle = channel.Title,
                                                Title = video.Title,
                                                Duration = video.Duration,
                                                PublishDate = video.PublishDate,
                                                ThumbnailUrl = video.ThumbnailUrl,
                                                VideoURL = video.VideoURL
                                            };
            return View("Index", await results.ToListAsync());

            /*public async Task<IActionResult> DisplayGroups(string id, string channelId, string groupingSelected, VideoItem clickedVideoItem)
        {
            if (User != null)
            {
                _info = GlobalVariables.UserLoginInfo[User.Identity.Name];
            }
            IQueryable<VideoItem> results = from video in _context.VideoItem
                                            join channel in _context.ChannelItem on video.ChannelId equals channel.Id
                                            where video.Title.Contains(clickedVideoItem.GroupingSelected) &&
                                            video.ChannelId.Equals(clickedVideoItem.ChannelId)
                                            select new VideoItem
                                            {
                                                ChannelTitle = channel.Title,
                                                Title = video.Title,
                                                Duration = video.Duration,
                                                PublishDate = video.PublishDate,
                                                ThumbnailUrl = video.ThumbnailUrl,
                                                VideoURL = video.VideoURL
                                            };
            //
            return View("Index", await results.ToListAsync());*/
        }
    }
}
