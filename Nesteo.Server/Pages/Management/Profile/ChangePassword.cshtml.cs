using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Pages.Management.Profile
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public ChangePasswordModel(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [MinLength(Constants.MinPasswordLength, ErrorMessage = "The new password must be at least {1} characters long.")]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "New password (again)")]
            [Compare(nameof(NewPassword), ErrorMessage = "The new passwords don't match.")]
            public string ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            UserEntity user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            StatusMessage = "Password changed successfully!";

            return RedirectToPage();
        }
    }
}
