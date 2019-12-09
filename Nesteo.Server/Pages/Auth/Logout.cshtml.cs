using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<UserEntity> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation("User logged out.");

            return RedirectToPage();
        }
    }
}
