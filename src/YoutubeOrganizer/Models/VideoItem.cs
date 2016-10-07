using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
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


        [NotMapped]
        public string DisplayDuration
        {
            get { return GetDuration(); }
            set { Duration = value; }
        }

        public string GetDuration()
        {
            if (Duration == null || Duration.Equals("")) return "";
            //pattern = P[n]Y[n]M[n]DT[n]H[n]M[n]S;
            var regex = new Regex(@"P(?<Year>\d+Y)?(?<Month>\d+M)?(?<Day>\d+D)?T(?<Hour>\d+H)?(?<Minute>\d+M)?(?<Second>\d+S)?");
            var match = regex.Match(Duration);
            var groups = match.Groups;
            int years = !groups["Year"].Value.Equals("") ? Convert.ToInt32(groups["Year"].Value.TrimEnd('Y')) : 0;
            int months = !groups["Month"].Value.Equals("") ? Convert.ToInt32(groups["Month"].Value.TrimEnd('M')) : 0;
            int days  = !groups["Day"].Value.Equals("") ? Convert.ToInt32(groups["Day"].Value.TrimEnd('D')) : 0;
            int hours = !groups["Hour"].Value.Equals("") ? Convert.ToInt32(groups["Hour"].Value.TrimEnd('H')) : 0;
            int mins  = !groups["Minute"].Value.Equals("") ? Convert.ToInt32(groups["Minute"].Value.TrimEnd('M')) : 0;
            int secs  = !groups["Second"].Value.Equals("") ? Convert.ToInt32(groups["Second"].Value.TrimEnd('S')) : 0;
            TimeSpan time = new TimeSpan((years*365+months*30+days)*24+hours,mins,secs);
            return time.TotalHours >= 1 ? time.ToString("hh\\:mm\\:ss") : time.ToString("mm\\:ss");
        }

    }
}
