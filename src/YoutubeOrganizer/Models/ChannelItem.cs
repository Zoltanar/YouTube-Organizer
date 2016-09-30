using System.ComponentModel.DataAnnotations;

namespace YoutubeOrganizer.Models
{
    public class ChannelItem
    {
        /// <summary> Database ID </summary>
        [Key]
        public int DatabaseId { get; set; }
        
        /// <summary>
        /// The ID that YouTube uses to uniquely identify the channel.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The channel's Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The ID of the playlist that contains the channel"s uploaded videos. Use the videos.insert
        ///method to upload new videos and the videos.delete method to delete previously
        ///uploaded videos.
        /// </summary>
        //[JsonProperty("uploads")]
        public string UploadPlaylist { get; set; }//=> ContentDetails.RelatedPlaylists.Uploads;
    }
}
