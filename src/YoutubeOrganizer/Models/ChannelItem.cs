using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

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
