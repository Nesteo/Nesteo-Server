using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// Retrieve all users
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<User> GetUsersAsync()
        {
            return _userService.GetAllAsync();
        }

        /// <summary>
        /// Retrieve a user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(string id)
        {
            // Retrieve user
            User user = await _userService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (user == null)
                return NotFound();

            return user;
        }
    }
}
