using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// A YouTube video.
    /// </summary>
    public class VideoItem
    {
        [Key]
        public int DatabaseId { get; set; }

        /// <summary>
        /// YouTube's unique identifier for this video.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// YouTube's unique identifier for this video's channel.
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// The title of the video.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The duration of the video.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Location of video's thumbnail.
        /// </summary>
        public string ThumbnailUrl { get; set; }
        
        /// <summary>
        /// Height of video's thumbnail.
        /// </summary>
        public long? ThumbnailHeight { get; set; }

        /// <summary>
        /// Width of video's thumbnail.
        /// </summary>
        public long? ThumbnailWidth { get; set; }

        /// <summary>
        /// Publish date of video.
        /// </summary>
        public DateTime? PublishDate { get; set; }


        public override string ToString() => $"Title= {Title} - ID= {Id}";
    }
}
