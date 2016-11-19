namespace YoutubeOrganizer.Models
{
    /// <summary>
    /// Group of videos found by the relationship between User and a channelId, a group of videos (grouped by matching regex in their titles).
    /// </summary>
    public class UserGroup
    {
        /// <summary>
        /// Unique User ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Unique YouTube Channel ID
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// User-defined name of group.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Regex string to use when grouping, if null then all videos will be part of the group.
        /// </summary>
        public string GroupingTemplate { get; set; }

        /// <summary>
        /// Create UserGroup from user, groupname and channel.
        /// </summary>
        /// <param name="userId">ProviderKey of user (by Google)</param>
        /// <param name="channelId">YouTube's unique channel identifier</param>
        /// <param name="groupName">Name of group</param>
        public UserGroup(string userId, string channelId, string groupName)
        {
            UserId = userId;
            ChannelId = channelId;
            GroupName = groupName;
        }

        /// <summary>
        /// Create UserGroup from user, groupname, channel and groupingTemplate.
        /// </summary>
        /// <param name="userId">ProviderKey of user (by Google)</param>
        /// <param name="channelId">YouTube's unique channel identifier</param>
        /// <param name="groupName">Name of group</param>
        /// <param name="groupingTemplate">String defining videos in group</param>
        public UserGroup(string userId, string channelId, string groupName,string groupingTemplate)
        {
            UserId = userId;
            ChannelId = channelId;
            GroupName = groupName;
            GroupingTemplate = groupingTemplate;
        }
    }
}
