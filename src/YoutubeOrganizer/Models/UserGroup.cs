using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// Constructor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        /// <param name="groupingTemplate"></param>
        /// <param name="groupName"></param>
        public UserGroup(string userId, string channelId, string groupingTemplate, string groupName)
        {
            UserId = userId;
            ChannelId = channelId;
            GroupingTemplate = groupingTemplate;
            GroupName = groupName;
        }
    }
}
