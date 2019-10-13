using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/auth")]
    public class AuthController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Retrieve information about the currently authenticated user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<User>> GetAuthenticatedUserAsync()
        {
            // Get the of the currently authenticated user
            string currentUserId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return NotFound();

            // Retrieve user
            User user = await _userService.FindByIdAsync(currentUserId, HttpContext.RequestAborted).ConfigureAwait(false);
            if (user == null)
                return NotFound();

            return user;
        }
    }
}
