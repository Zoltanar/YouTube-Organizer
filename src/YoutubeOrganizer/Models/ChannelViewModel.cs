namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// ViewModel for channel details pages.
    /// </summary>
    public class ChannelViewModel
    {
        /// <summary>
        /// Contains details about channel.
        /// </summary>
        public ChannelItem Channel { get; set; }

        /// <summary>
        /// Contains list of videos.
        /// </summary>
        public PagedVideoList VideoList { get; set; }

        /// <summary>
        /// ViewModel for channel details pages.
        /// </summary>
        /// <param name="channel">Channel details</param>
        /// <param name="videoList">List of videos</param>
        public ChannelViewModel(ChannelItem channel, PagedVideoList videoList)
        {
            Channel = channel;
            VideoList = videoList;
        }

        // ReSharper disable once UnusedMember.Global
        public ChannelViewModel() { }
    }
}
