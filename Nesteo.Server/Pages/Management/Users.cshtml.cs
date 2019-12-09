using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Pages.Management
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<UserEntity> _userManager;

        public UsersModel(UserManager<UserEntity> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public List<UserEntity> Users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel CreateInput { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Prompt = "Username")]
            public string UserName { get; set; }

            [Required]
            [Display(Prompt = "First name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Prompt = "Last name")]
            public string LastName { get; set; }

            [EmailAddress]
            [Display(Prompt = "Mail address (optional)")]
            public string Email { get; set; }

            [Phone]
            [Display(Prompt = "Phone number (optional)")]
            public string PhoneNumber { get; set; }

            [Required]
            [MinLength(Constants.MinPasswordLength, ErrorMessage = "The new password must be at least {1} characters long.")]
            [DataType(DataType.Password)]
            [Display(Prompt = "Password")]
            public string Password { get; set; }
        }

        private async Task LoadAsync()
        {
            Users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync().ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await LoadAsync().ConfigureAwait(false);

            if (!ModelState.IsValid)
                return Page();

            IdentityResult result = await _userManager.CreateAsync(new UserEntity {
                                                                       UserName = CreateInput.UserName,
                                                                       FirstName = CreateInput.FirstName,
                                                                       LastName = CreateInput.LastName,
                                                                       Email = CreateInput.Email,
                                                                       PhoneNumber = CreateInput.PhoneNumber
                                                                   },
                                                                   CreateInput.Password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            StatusMessage = "User created successfully.";

            return RedirectToPage();
        }
    }
}
