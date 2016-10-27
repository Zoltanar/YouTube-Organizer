namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// Relationship between a video and a user.
    /// </summary>
    public class UserVideo
    {
        /// <summary>
        /// Unique YouTube video ID.
        /// </summary>
        public string VideoId { get; set; }

        /// <summary>
        /// ProviderKey of User to whom this object belongs to.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// User's status determining if he's watched the video.
        /// </summary>
        public bool Watched { get; set; }
    }
}
