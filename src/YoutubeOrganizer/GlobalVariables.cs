using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace YoutubeOrganizer
{
    /// <summary>
    /// Contains static variables that can be accessed anywhere.
    /// </summary>
    public static class GlobalVariables
    {
        /// <summary>
        /// You can set the location of your google secrets file here, you can obtain it from https://console.developers.google.com/apis/
        /// </summary>
        public const string SecretsFile = "C:\\client_secrets.json";
    }
}
