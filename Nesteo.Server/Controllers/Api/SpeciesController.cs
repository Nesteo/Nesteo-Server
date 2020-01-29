using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/species")]
    public class SpeciesController : ApiControllerBase
    {
        private readonly ISpeciesService _speciesService;

        public SpeciesController(ISpeciesService speciesService)
        {
            _speciesService = speciesService ?? throw new ArgumentNullException(nameof(speciesService));
        }

        /// <summary>
        /// Retrieve all species
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<Species> GetSpeciesAsync() => _speciesService.GetAllAsync();

        /// <summary>
        /// Retrieve a species by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Species>> GetSpeciesByIdAsync(int id)
        {
            // Retrieve species
            Species species = await _speciesService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (species == null)
                return NotFound();

            return species;
        }
    }
}
