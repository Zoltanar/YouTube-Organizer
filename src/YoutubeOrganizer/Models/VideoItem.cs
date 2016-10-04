using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// A YouTube video.
    /// </summary>
    public class VideoItem
    {

        /// <summary>
        /// YouTube's unique identifier for this video.
        /// </summary>
        [Key]
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
        [Display(Name = "Thumbnail")]
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
        [Display(Name = "Published")]
        public DateTime? PublishDate { get; set; }

        [NotMapped]
        public bool Watched { get; set; }

        [NotMapped, Display(Name = "Channel")]
        public string ChannelTitle { get; set; }

        [Display(Name = "Link")]
        public string VideoURL { get; set; }

        [NotMapped]
        public SelectList Groupings { get; set; }

        [NotMapped]
        public string GroupingSelected { get; set; }

        public override string ToString() => $"Title= {Title} - ID= {Id}";


    }
}
