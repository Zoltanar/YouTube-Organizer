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

        /// <summary>
        /// Link to video.
        /// </summary>
        [Display(Name = "Link")]
        public string VideoURL { get; set; }

        /// <summary>
        /// Whether the user has watched the video.
        /// </summary>
        [NotMapped]
        public bool Watched { get; set; }

        /// <summary>
        /// Title of channel that owns this video.
        /// </summary>
        [NotMapped, Display(Name = "Channel")]
        public string ChannelTitle { get; set; }

        /// <summary>
        /// Possible groupings for the video.
        /// </summary>
        [NotMapped]
        public SelectList Groupings { get; set; }

        /// <summary>
        /// Grouping that was selected.
        /// </summary>
        [NotMapped]
        public string GroupingSelected { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"Title= {Title} - ID= {Id}";

        /// <summary>
        /// Duration as it is shown to users.
        /// </summary>
        [NotMapped]
        public string DisplayDuration => GetDuration();

        /// <summary>
        /// Timescale-aware string, time since video was published.
        /// </summary>
        [NotMapped]
        public string TimeSince => GetTimeSince();

        /// <summary>
        /// Return duration of video as it should be displayed to user, converts from ISO 8601 date (PnYnMnDTnHnMnS).
        /// </summary>
        public string GetDuration()
        {
            if (Duration == null || Duration.Equals("")) return "";
            //pattern = P[n]Y[n]M[n]DT[n]H[n]M[n]S;
            var regex = new Regex(@"P(?<Year>\d+Y)?(?<Month>\d+M)?(?<Day>\d+D)?T(?<Hour>\d+H)?(?<Minute>\d+M)?(?<Second>\d+S)?");
            var match = regex.Match(Duration);
            var groups = match.Groups;
            int years = !groups["Year"].Value.Equals("") ? Convert.ToInt32(groups["Year"].Value.TrimEnd('Y')) : 0;
            int months = !groups["Month"].Value.Equals("") ? Convert.ToInt32(groups["Month"].Value.TrimEnd('M')) : 0;
            int days = !groups["Day"].Value.Equals("") ? Convert.ToInt32(groups["Day"].Value.TrimEnd('D')) : 0;
            int hours = !groups["Hour"].Value.Equals("") ? Convert.ToInt32(groups["Hour"].Value.TrimEnd('H')) : 0;
            int mins = !groups["Minute"].Value.Equals("") ? Convert.ToInt32(groups["Minute"].Value.TrimEnd('M')) : 0;
            int secs = !groups["Second"].Value.Equals("") ? Convert.ToInt32(groups["Second"].Value.TrimEnd('S')) : 0;
            TimeSpan time = new TimeSpan((years * 365 + months * 30 + days) * 24 + hours, mins, secs);
            return time.TotalHours >= 1 ? time.ToString("hh\\:mm\\:ss") : time.ToString("mm\\:ss");
        }


        /// <summary>
        /// Get time since video was published, uses different word based on the length (days, hours, etc).
        /// </summary>
        /// <returns></returns>
        public string GetTimeSince()
        {
            if (PublishDate > DateTime.UtcNow || PublishDate == null) return "Future"; //shouldnt happen
            TimeSpan timeSince = DateTime.UtcNow - (DateTime)PublishDate;
            if (timeSince.TotalDays > 365.25 * 2) return Math.Floor(timeSince.TotalDays / 365) + " years ago."; //2+ years
            if (timeSince.TotalDays > 365.25) return "1 year ago.";
            if (timeSince.TotalDays > 30.5 * 2) return Math.Floor(timeSince.TotalDays / 30.5) + " months ago."; //2+ months
            if (timeSince.TotalDays > 7 * 2) return Math.Floor(timeSince.TotalDays / 7) + " weeks ago."; //2+ weeks
            if (timeSince.TotalDays > 7 * 2) return "1 week ago.";
            if (timeSince.TotalDays > 2) return Math.Floor(timeSince.TotalDays) + " days ago."; //2+ days
            if (timeSince.TotalDays > 1) return "1 day ago.";
            if (timeSince.TotalHours > 2) return Math.Floor(timeSince.TotalHours) + " hours ago."; //2+ hours
            if (timeSince.TotalHours > 1) return "1 hour ago.";
            if (timeSince.TotalMinutes > 2) return Math.Floor(timeSince.TotalMinutes) + " minutes ago."; //2+ minutes
            return "just now.";

        }
    }
}
