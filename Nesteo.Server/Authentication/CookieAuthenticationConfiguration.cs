using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Nesteo.Server.Authentication
{
    public class CookieAuthenticationConfiguration : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public void Configure(CookieAuthenticationOptions options) { }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name != IdentityConstants.ApplicationScheme)
                throw new InvalidOperationException($"Cannot configure unknown cookie authentication scheme {name}");

            options.Cookie.Name = "sessionToken";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/access-denied";
            options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            options.SlidingExpiration = true;
        }
    }
}
