using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Pages.Management
{
    public class EditUserModel : PageModel
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public EditUserModel(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
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

            [MinLength(Constants.MinPasswordLength, ErrorMessage = "The new password must be at least {1} characters long.")]
            [DataType(DataType.Password)]
            [Display(Name = "New password", Prompt = "Leave blank to keep the old one")]
            public string NewPassword { get; set; }
        }

        private async Task LoadAsync(UserEntity user)
        {
            UserName = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);
            Input = new InputModel { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, PhoneNumber = user.PhoneNumber };
        }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            UserEntity user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

            await LoadAsync(user).ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userName)
        {
            UserEntity user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

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
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await LoadAsync(user).ConfigureAwait(false);
                return Page();
            }

            if (Input.NewPassword != null)
            {
                await _userManager.RemovePasswordAsync(user).ConfigureAwait(false);
                result = await _userManager.AddPasswordAsync(user, Input.NewPassword).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await LoadAsync(user).ConfigureAwait(false);
                    return Page();
                }
            }

            if (_userManager.GetUserId(User) == user.Id)
                await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            StatusMessage = "User updated successfully.";

            await LoadAsync(user).ConfigureAwait(false);
            return RedirectToPage("/Management/Users");
        }
    }
}
