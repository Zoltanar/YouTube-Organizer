using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

    public class ChannelItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExternalLoginInfo _info;


        public ChannelItemsController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        private UserCredential GetCredentialForApi()
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer();
            using (var stream = new FileStream("C:\\client_secrets.json", FileMode.Open, FileAccess.Read))
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

        /// <summary>
        /// Get Subscriptions for currently logged in user.
        /// </summary>
        public async Task<IActionResult> GetSubscriptions()
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

            //prepare request
            var request = youtubeService.Subscriptions.List("snippet");
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

            var channelRequest = youtubeService.Channels.List("snippet,ContentDetails");
            var channelList = new List<ChannelItem>();
            int taken = 0;
            do
            {
                channelRequest.Id = string.Join(",", croppedChannelList.Skip(taken).Take(50));
                ChannelListResponse channelResponse = await channelRequest.ExecuteAsync();
                channelList.AddRange(MakeChannelList(channelResponse.Items));
                taken += 50;
            } while (taken < croppedChannelList.Count);
            //
            List<SubscriptionItem> subList = channelIDs.Select(channelID => new SubscriptionItem {ChannelId = channelID, UserId = _info.ProviderKey }).ToList();
            _context.AddRange(channelList);
            _context.AddRange(subList);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: ChannelItems
        public async Task<IActionResult> Index()
        {
            if (User?.Identity.Name != null)
            {
                _info = GlobalVariables.UserLoginInfo.Keys.Contains(User.Identity.Name)? GlobalVariables.UserLoginInfo[User.Identity.Name] : null;
            }
            if (_info == null || !_info.Principal.Identity.IsAuthenticated) return View(new List<ChannelItem>()); //TODO should return not logged in or not logged in with google
            List<SubscriptionItem> subs = _context.SubscriptionItem.Where(x=>x.UserId.Equals(_info.ProviderKey)).ToList();

            return View(await _context.ChannelItem.Where(channel=>subs.Find(sub=>sub.ChannelId.Equals(channel.Id))!= null).ToListAsync());
        }

        private static List<ChannelItem> MakeChannelList(IEnumerable<Channel> channelResponseItems)
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.DatabaseId == id);
            if (channelItem == null)
            {
                return NotFound();
            }

            return View(channelItem);
        }

        // GET: ChannelItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChannelItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatabaseId,Id,UploadPlaylist")] ChannelItem channelItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(channelItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(channelItem);
        }

        // GET: ChannelItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.DatabaseId == id);
            if (channelItem == null)
            {
                return NotFound();
            }
            return View(channelItem);
        }

        // POST: ChannelItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DatabaseId,Id,UploadPlaylist")] ChannelItem channelItem)
        {
            if (id != channelItem.DatabaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(channelItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelItemExists(channelItem.DatabaseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(channelItem);
        }

        // GET: ChannelItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.DatabaseId == id);
            if (channelItem == null)
            {
                return NotFound();
            }

            return View(channelItem);
        }

        // POST: ChannelItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var channelItem = await _context.ChannelItem.SingleOrDefaultAsync(m => m.DatabaseId == id);
            _context.ChannelItem.Remove(channelItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ChannelItemExists(int id)
        {
            return _context.ChannelItem.Any(e => e.DatabaseId == id);
        }
        
    }
}
