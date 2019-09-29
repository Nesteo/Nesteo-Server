using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/owners")]
    public class OwnersController : ApiControllerBase
    {
        /// <summary>
        /// Retrieves all owners
        /// </summary>
        // TODO: Use IAsyncEnumerable<> after EF Core upgrade
        [HttpGet]
        public Task<ActionResult<ICollection<Owner>>> GetOwnersAsync()
        {
            return Task.FromResult<ActionResult<ICollection<Owner>>>(new List<Owner> { new Owner { Id = 0, Name = "He-who-must-not-be-named" } });
        }

        /// <summary>
        /// Retrieves an owner by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult<Owner>> GetOwnerByIdAsync(int id)
        {
            if (id != 0)
                return Task.FromResult<ActionResult<Owner>>(NotFound());

            return Task.FromResult<ActionResult<Owner>>(new Owner { Id = 0, Name = "He-who-must-not-be-named" });
        }
    }
}
