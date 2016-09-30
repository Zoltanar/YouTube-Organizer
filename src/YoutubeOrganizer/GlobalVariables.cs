using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace YoutubeOrganizer
{
    public static class GlobalVariables
    {
        public static Dictionary<string, ExternalLoginInfo> UserLoginInfo { get; set; }
        //You can set the location of your google secrets file here, you can obtain it from https://console.developers.google.com/apis/
        public const string SecretsFile = "C:\\client_secrets.json";


    }
}
