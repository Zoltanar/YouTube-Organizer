using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeOrganizer.Models
{
    public class ChannelItem
    {
        /// <summary>
        /// The ID that YouTube uses to uniquely identify the channel.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// The channel's Title
        /// </summary>
        public string ChannelTitle { get; set; }

        /// <summary>
        /// The ID of the playlist that contains the channel"s uploaded videos. Use the videos.insert
        ///method to upload new videos and the videos.delete method to delete previously
        ///uploaded videos.
        /// </summary>

        [Display(Name = "Upload Playlist")]
        public string UploadPlaylist { get; set; }//=> ContentDetails.RelatedPlaylists.Uploads;

        [Display(Name = "Number of videos")]
        public int NumberOfVideos { get; set; }

        [NotMapped]
        public string ChannelURL { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"Title= {ChannelTitle} - ID= {Id}";
    }
}
