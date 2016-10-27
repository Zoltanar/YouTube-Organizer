using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YoutubeOrganizer.Models;

namespace YoutubeOrganizer.Data
{
    /// <summary>
    /// Context holding database of user-related objects, does not contain users.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserVideo>().HasKey(t => new { t.UserID, t.VideoId });
            builder.Entity<UserGroup>().HasKey(t => new { t.UserId, t.ChannelId, t.GroupingTemplate });
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        /// <summary>
        /// DB Table containing YouTube Channels.
        /// </summary>
        public DbSet<ChannelItem> ChannelItem { get; set; }
        /// <summary>
        /// DB Table containing User Subscriptions.
        /// </summary>
        public DbSet<SubscriptionItem> SubscriptionItem { get; set; }
        /// <summary>
        /// DB Table containing YouTube Videos.
        /// </summary>
        public DbSet<VideoItem> VideoItem { get; set; }
        /// <summary>
        /// DB Table containing relationship between users and videos.
        /// </summary>
        public DbSet<UserVideo> UserVideo { get; set; }
        /// <summary>
        /// DB Table containing relationship between users and groups of videos.
        /// </summary>
        public DbSet<UserGroup> UserGroup { get; set; }
    }
}
