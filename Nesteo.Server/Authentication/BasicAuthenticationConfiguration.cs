using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nesteo.Server.Data.Entities.Identity;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace Nesteo.Server.Authentication
{
    public class BasicAuthenticationConfiguration : IConfigureNamedOptions<BasicAuthenticationOptions>
    {
        public void Configure(BasicAuthenticationOptions options) { }

        public void Configure(string name, BasicAuthenticationOptions options)
        {
            if (name != BasicAuthenticationDefaults.AuthenticationScheme)
                throw new InvalidOperationException($"Cannot configure unknown basic authentication scheme {name}");

            options.Realm = "Nesteo API";
            options.Events = new BasicAuthenticationEvents { OnValidatePrincipal = ValidatePrincipalAsync };
        }

        private async Task ValidatePrincipalAsync(ValidatePrincipalContext context)
        {
            // Try sign in
            SignInResult result = await SignInAsync(context.HttpContext, context.UserName, context.Password, true, BasicAuthenticationDefaults.AuthenticationScheme)
                .ConfigureAwait(false);

            // Check result
            if (result.Succeeded)
                context.Principal = context.HttpContext.User;
            else if (result.IsLockedOut)
                context.AuthenticationFailMessage = "Account locked.";
            else
                context.AuthenticationFailMessage = "Authentication failed.";
        }

        private async Task<SignInResult> SignInAsync(HttpContext httpContext, string userName, string password, bool lockoutOnFailure, string authenticationScheme)
        {
            // Get user manager
            UserManager<UserEntity> userManager = httpContext.RequestServices.GetRequiredService<UserManager<UserEntity>>();

            // Get user
            UserEntity userEntity = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (userEntity == null)
                return SignInResult.Failed;

            // Get sign in manager. We can't use most of it's methods, because they are quite cookie-specific, but a few of them are useful here.
            SignInManager<UserEntity> signInManager = httpContext.RequestServices.GetRequiredService<SignInManager<UserEntity>>();

            // Check password
            SignInResult checkPasswordResult = await signInManager.CheckPasswordSignInAsync(userEntity, password, lockoutOnFailure).ConfigureAwait(false);
            if (!checkPasswordResult.Succeeded)
                return checkPasswordResult;

            // Create user principal
            ClaimsPrincipal userPrincipal = await signInManager.CreateUserPrincipalAsync(userEntity).ConfigureAwait(false);
            userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationScheme));

            // Sign the http context in
            httpContext.User = userPrincipal;

            // Successfully signed in
            return SignInResult.Success;
        }
    }
}
