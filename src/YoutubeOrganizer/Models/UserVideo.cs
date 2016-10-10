namespace YoutubeOrganizer.Models
{
    public class UserVideo
    {
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
