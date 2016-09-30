using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeOrganizer.Models
{
    public class SubscriptionItem
    {
        [Key]
        public int Id { get; set; }

        //couldnt get composite key to work

        public string ChannelId { get; set; }
        
        public string UserId { get; set; }
    }
}
