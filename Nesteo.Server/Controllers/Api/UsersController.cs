using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/users")]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        // TODO: Use IAsyncEnumerable<> after EF Core upgrade
        [HttpGet]
        public async Task<ActionResult<ICollection<User>>> GetUsersAsync()
        {
            return Ok(await _userService.GetAllUsersAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Retrieves a user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(string id)
        {
            // Retrieve user
            User user = await _userService.FindUserByIdAsync(id).ConfigureAwait(false);
            if (user == null)
                return NotFound();

            return user;
        }
    }
}
