using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Pages.Management.Profile
{
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public EditProfileModel(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [EmailAddress]
            [Display(Name = "Mail address (optional)")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number (optional)")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(UserEntity user)
        {
            UserName = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);
            Input = new InputModel { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, PhoneNumber = user.PhoneNumber };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserEntity user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await LoadAsync(user).ConfigureAwait(false);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserEntity user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (!ModelState.IsValid)
            {
                await LoadAsync(user).ConfigureAwait(false);
                return Page();
            }

            if (Input.FirstName != user.FirstName)
                user.FirstName = Input.FirstName;

            if (Input.LastName != user.LastName)
                user.LastName = Input.LastName;

            if (Input.Email != user.Email)
                user.Email = Input.Email;

            if (Input.PhoneNumber != user.PhoneNumber)
                user.PhoneNumber = Input.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unexpected error occurred while updating the user with ID '{user.Id}'.");

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            StatusMessage = "Profile updated successfully.";

            return RedirectToPage();
        }
    }
}
