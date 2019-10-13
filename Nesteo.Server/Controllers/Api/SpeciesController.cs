using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/species")]
    public class SpeciesController : ApiControllerBase
    {
        /// <summary>
        /// Retrieve all species
        /// </summary>
        // TODO: Use IAsyncEnumerable<>
        [HttpGet]
        public Task<ActionResult<ICollection<Species>>> GetSpeciesAsync()
        {
            return Task.FromResult<ActionResult<ICollection<Species>>>(new List<Species> { new Species { Id = 0, Name = "Dodo" } });
        }

        /// <summary>
        /// Retrieve a species by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult<Species>> GetSpeciesByIdAsync(int id)
        {
            if (id != 0)
                return Task.FromResult<ActionResult<Species>>(NotFound());

            return Task.FromResult<ActionResult<Species>>(new Species { Id = 0, Name = "Dodo" });
        }
    }
}
