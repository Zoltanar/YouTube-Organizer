using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NuGet.Protocol.Core.v3;

namespace YoutubeOrganizer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public string Tokens { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

        public void SetLoginInfo(ExternalLoginInfo info)
        {
            LoginProvider = info.LoginProvider;
            ProviderKey = info.ProviderKey;
            Tokens = info.AuthenticationTokens?.ToJson();
        }

        public ShortLoginInfo GetLoginInfo()
        {
            if (Tokens.Equals("") || LoginProvider.Equals("") || ProviderKey.Equals("")) return null;
            return new ShortLoginInfo
            {
                AuthenticationTokens = Tokens.FromJson<IEnumerable<AuthenticationToken>>(),
                LoginProvider = LoginProvider,
                ProviderKey = ProviderKey
            };
        }

    }

    public class ShortLoginInfo
    {
        public IEnumerable<AuthenticationToken> AuthenticationTokens { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

    }
}
