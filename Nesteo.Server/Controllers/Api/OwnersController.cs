using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/owners")]
    public class OwnersController : ApiControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService ?? throw new ArgumentNullException(nameof(ownerService));
        }

        /// <summary>
        /// Retrieves all owners
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<Owner> GetOwnersAsync()
        {
            return _ownerService.GetAllAsync();
        }

        /// <summary>
        /// Retrieves an owner by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetOwnerByIdAsync(int id)
        {
            // Retrieve owner
            Owner owner = await _ownerService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (owner == null)
                return NotFound();

            return owner;
        }
    }
}
