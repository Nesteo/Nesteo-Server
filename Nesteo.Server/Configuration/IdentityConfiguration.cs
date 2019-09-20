using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Nesteo.Server.Configuration
{
    public class IdentityConfiguration : IConfigureOptions<IdentityOptions>
    {
        public void Configure(IdentityOptions options)
        {
            // Password constraints
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = Constants.MinPasswordLength;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        }
    }
}
