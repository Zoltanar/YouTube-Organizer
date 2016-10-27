using System.ComponentModel.DataAnnotations;

namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// Subscription by a user.
    /// </summary>
    public class SubscriptionItem
    {
        /// <summary>
        /// Database ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        //TODO couldnt get composite key to work

        /// <summary>
        /// Unique YouTube channel ID
        /// </summary>
        public string ChannelId { get; set; }
        
        /// <summary>
        /// Unique Google Provider Key.
        /// </summary>
        public string UserId { get; set; }
    }
}
