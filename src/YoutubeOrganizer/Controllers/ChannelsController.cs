using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YoutubeOrganizer.Data;
using YoutubeOrganizer.Models;
using Microsoft.AspNetCore.Identity;

namespace YoutubeOrganizer.Controllers
{

    public class ChannelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExternalLoginInfo _info;
        private const int YouTubeMaxResults = 50;
        private const string YouTubeShortURL = "https://youtu.be/";
        private const string YouTubeChannelPrepend = "https://youtube.com/channel/";


        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public ChannelsController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        /// <summary>
        /// Get user credential for currently logged in user.
        /// </summary>
        /// <returns>Google User Credential for logged in user</returns>
        private UserCredential GetCredentialForApi()
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer();
            using (var stream = new FileStream(GlobalVariables.SecretsFile, FileMode.Open, FileAccess.Read))
            {
                ClientSecrets secrets = GoogleClientSecrets.Load(stream).Secrets;
                initializer.ClientSecrets = new ClientSecrets
                {
                    ClientId = secrets.ClientId,
                    ClientSecret = secrets.ClientSecret,
                };
            }
            initializer.Scopes = new[] { YouTubeService.Scope.Youtube };
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            //ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(); //this doesnt work for some reason
            TokenResponse token = new TokenResponse { RefreshToken = _info.AuthenticationTokens.First(x => x.Name == "refresh_token").Value };
            return new UserCredential(flow, _info.ProviderKey, token);
        }

        private YouTubeService CreateYouTubeService()
        {
            if (User != null)
            {
                _info = GlobalVariables.UserLoginInfo[User.Identity.Name];
            }
            if (_info == null || !_info.LoginProvider.Equals("Google")) return null;
            //get user credential and initialize youtube service
            var credential = GetCredentialForApi();
            return new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "YoutubeOrganizer"
            });
        }

        /// <summary>
        /// Get videos from the user's subscribed channels.
        /// </summary>
        public async Task<IActionResult> GetVideos()
        {
            if (User != null)
            {
                _info = GlobalVariables.UserLoginInfo[User.Identity.Name];
            }
            if (_info == null || !_info.LoginProvider.Equals("Google")) return RedirectToAction("Index"); //TODO should return not logged in through google
            //get user credential and initialize youtube service
            var credential = GetCredentialForApi();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "YoutubeOrganizer"
            });

            List<SubscriptionItem> subs = _context.SubscriptionItem.Where(x => x.UserId.Equals(_info.ProviderKey)).ToList();
            var presentVideos = await _context.VideoItem.Select(x => x.Id).ToListAsync();
            var channels = _context.ChannelItem.Where(channel => subs.Find(sub => sub.ChannelId.Equals(channel.Id)) != null).ToList();
            await FetchVideoIdsByChannel(youtubeService, channels, presentVideos);
            //prepare request
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get videos from the selected channel.
        /// </summary>
        /// <param name="id">Database ID of channel</param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateVideos(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.Id.Equals(id));
            if (channelItem == null)
            {
                return NotFound();
            }
            var presentVideos = await _context.VideoItem.Select(x => x.Id).ToListAsync();
            var youTubeService = CreateYouTubeService();
            if(youTubeService == null) return RedirectToAction("Index"); //TODO should return error
            await FetchVideoIdsByChannel(youTubeService, new[] { channelItem }, presentVideos);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get videos from the selected channel.
        /// </summary>
        /// <param name="id">Database ID of channel</param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateVideosChannelPage(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.Id.Equals(id));
            if (channelItem == null)
            {
                return NotFound();
            }
            var presentVideos = await _context.VideoItem.Select(x => x.Id).ToListAsync();
            var youTubeService = CreateYouTubeService();
            await FetchVideoIdsByChannel(youTubeService, new[] { channelItem }, presentVideos);
            return RedirectToAction($"Details/{channelItem.Id}");
        }

        /// <summary>
        /// Fetch IDs of a channel's videos by using the channel's uploads playlist.
        /// </summary>
        /// <param name="youTubeService">Authenticated Service to be used</param>
        /// <param name="channels">List of channels</param>
        /// <param name="presentVideos">List of videos already stored</param>
        /// <returns></returns>
        private async Task FetchVideoIdsByChannel(YouTubeService youTubeService, IEnumerable<ChannelItem> channels, List<string> presentVideos)
        {
            var request = youTubeService.PlaylistItems.List("contentDetails");
            request.MaxResults = YouTubeMaxResults;
            foreach (var channel in channels)
            {
                var items = new List<PlaylistItem>();
                request.PlaylistId = channel.UploadPlaylist;
                var response = await request.ExecuteAsync();
                items.AddRange(response.Items);
                while (response.NextPageToken != null)
                {
                    request.PageToken = response.NextPageToken;
                    response = await request.ExecuteAsync();
                    items.AddRange(response.Items);
                }
                var videosToGet = items.Select(x => x.ContentDetails.VideoId).Except(presentVideos).ToArray();
                await GetChannelVideos(youTubeService, channel, videosToGet);
            }
        }

        private async Task GetChannelVideos(YouTubeService youTubeService, ChannelItem channel, string[] videos)
        {
            if (videos.Length == 0) return;
            var request = youTubeService.Videos.List("snippet,contentDetails");
            request.MaxResults = YouTubeMaxResults;
            var videoList = new List<VideoItem>();
            int taken = 0;
            do
            {
                request.Id = string.Join(",", videos.Skip(taken).Take(YouTubeMaxResults));
                var response = await request.ExecuteAsync();
                videoList.AddRange(MakeVideoList(channel.Id, response.Items));
                taken += YouTubeMaxResults;
            } while (taken < videos.Length);
            _context.VideoItem.AddRange(videoList);
            var channelEntity = await _context.ChannelItem.SingleOrDefaultAsync(x => x.Id.Equals(channel.Id));
            channelEntity.NumberOfVideos += videoList.Count;
            _context.Update(channelEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Make list of VideoItem from list of Video.
        /// </summary>
        /// <param name="channelId">ID of the Channel to who the videos belong</param>
        /// <param name="videos">List of videos</param>
        /// <returns></returns>
        private static IEnumerable<VideoItem> MakeVideoList(string channelId, IEnumerable<Video> videos)
        {
            //TODO can this be done through simply casting the items?
            var list = new List<VideoItem>();
            foreach (var video in videos)
            {
                list.Add(new VideoItem
                {
                    Id = video.Id,
                    ChannelId = channelId,
                    Title = video.Snippet.Title,
                    Duration = video.ContentDetails.Duration,
                    PublishDate = video.Snippet.PublishedAt,
                    ThumbnailUrl = video.Snippet.Thumbnails.Default__.Url,
                    ThumbnailHeight = video.Snippet.Thumbnails.Default__.Height,
                    ThumbnailWidth = video.Snippet.Thumbnails.Default__.Width
                });
            }
            return list;
        }

        /// <summary>
        /// Get Subscriptions for currently logged in user.
        /// </summary>
        public async Task<IActionResult> GetSubscriptions()
        {
            var youTubeService = CreateYouTubeService();
            if (youTubeService == null) return RedirectToAction("Index"); //TODO should return not logged in through google
            //prepare request
            var request = youTubeService.Subscriptions.List("snippet");
            request.Mine = true;
            request.MaxResults = 50;
            request.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
            SubscriptionListResponse response = await request.ExecuteAsync();
            if (response.Items.Count == 0) return RedirectToAction("Index"); //TODO should return no subs found
            List<string> channelIDs = response.Items.Select(x => x.Snippet.ResourceId.ChannelId).ToList();
            //repeat request while next page is available
            while (response.NextPageToken != null)
            {
                request.PageToken = response.NextPageToken;
                response = await request.ExecuteAsync();
                channelIDs.AddRange(response.Items.Select(x => x.Snippet.ResourceId.ChannelId));
            }

            //remove channels already in local database from list to fetch
            DbSet<ChannelItem> presentChannels = _context.ChannelItem;
            List<string> croppedChannelList = channelIDs.Except(presentChannels.Select(x => x.Id)).ToList();

            if (croppedChannelList.Count != 0)
            {
                var channelRequest = youTubeService.Channels.List("snippet,ContentDetails");
                var channelList = new List<ChannelItem>();
                int taken = 0;
                do
                {
                    channelRequest.Id = string.Join(",", croppedChannelList.Skip(taken).Take(50));
                    ChannelListResponse channelResponse = await channelRequest.ExecuteAsync();
                    channelList.AddRange(MakeChannelList(channelResponse.Items));
                    taken += 50;
                } while (taken < croppedChannelList.Count);
                _context.ChannelItem.AddRange(channelList);
            }
            SubscriptionItem[] subList = channelIDs.Select(channelID => new SubscriptionItem { ChannelId = channelID, UserId = _info.ProviderKey }).ToArray(); //list of user's current subs
            IQueryable<SubscriptionItem> oldSubList = _context.SubscriptionItem.Where(x => x.UserId.Equals(_info.ProviderKey)); //list of user's subs in db
            IEnumerable<SubscriptionItem> newSubList = subList.Except(oldSubList); //list of subs that are not in db
            IQueryable<SubscriptionItem> deletedSubList = oldSubList.Except(subList); //list of subs in db that are no longer user's subs
            _context.SubscriptionItem.AddRange(newSubList);
            _context.SubscriptionItem.RemoveRange(deletedSubList);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: ChannelItems
        public async Task<IActionResult> Index()
        {
            if (User?.Identity.Name != null)
            {
                _info = GlobalVariables.UserLoginInfo.Keys.Contains(User.Identity.Name) ? GlobalVariables.UserLoginInfo[User.Identity.Name] : null;
            }
            if (_info == null) return View(new List<ChannelItem>()); //TODO should return not logged in or not logged in with google
            List<SubscriptionItem> subs = await _context.SubscriptionItem.Where(x => x.UserId.Equals(_info.ProviderKey)).ToListAsync();

            return View(_context.ChannelItem.Where(channel => subs.Find(sub => sub.ChannelId.Equals(channel.Id)) != null));
        }

        private static IEnumerable<ChannelItem> MakeChannelList(IEnumerable<Channel> channelResponseItems)
        {
            List<ChannelItem> list = new List<ChannelItem>();
            foreach (var channel in channelResponseItems)
            {
                list.Add(new ChannelItem
                {
                    Id = channel.Id,
                    Title = channel.Snippet.Title,
                    UploadPlaylist = channel.ContentDetails.RelatedPlaylists.Uploads
                });
            }
            return list;
        }

        // GET: ChannelItems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.Id.Equals(id));
            if (channelItem == null)
            {
                return NotFound();
            }
            channelItem.ChannelURL = YouTubeChannelPrepend + channelItem.Id;
            List<VideoItem> channelVideos = await _context.VideoItem.Where(video => video.ChannelId.Equals(id)).
                OrderByDescending(video => video.PublishDate).ToListAsync();
            return View(new Tuple<ChannelItem, IEnumerable<VideoItem>>(channelItem, channelVideos));
        }


        public async Task<IActionResult> History()
        {
            if (User != null)
            {
                _info = GlobalVariables.UserLoginInfo[User.Identity.Name];
            }
            if (_info == null) return RedirectToAction("Index"); //TODO should return not logged in
            List<string> videoIDs = await _context.UserVideo.Where(x => x.UserID.Equals(_info.ProviderKey) && x.Watched).Select(x => x.VideoId).ToListAsync();
            List<VideoItem> videos = await _context.VideoItem.Where(x => videoIDs.Contains(x.Id)).OrderByDescending(x => x.PublishDate).ToListAsync();

            var channels = await _context.ChannelItem.ToListAsync();
            IEnumerable<VideoItem> listForViewing = videos.Join(channels, video => video.ChannelId, channel => channel.Id, (video, channel) => new VideoItem
            {
                ChannelTitle = channel.Title,
                Title = video.Title,
                Duration = video.Duration,
                PublishDate = video.PublishDate,
                ThumbnailUrl = video.ThumbnailUrl,
                VideoURL = video.VideoURL
            });
            return View(listForViewing);

        }


        /// <summary>
        /// DO NOT USE. Get User's watched videos. Currently returns zero items due to YouTube API changes. 
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        public async Task<IActionResult> GetHistoryList()
        {
            return NotFound();

            var youTubeService = CreateYouTubeService();
            var request = youTubeService.PlaylistItems.List("contentDetails");
            request.PlaylistId = "HL";
            var response = await request.ExecuteAsync();
            if (response.Items.Count == 0) return RedirectToAction("Index");
            //else etc
            return RedirectToAction("Index");
        }

        public async Task<string> GetUserIdYouTube(YouTubeService youTubeService)
        {
            var request = youTubeService.Channels.List("id");
            request.Mine = true;
            var response = await request.ExecuteAsync();
            return response.Items.FirstOrDefault().Id;
        }
    }
}
