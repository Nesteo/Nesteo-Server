using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/regions")]
    public class RegionsController : ApiControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService ?? throw new ArgumentNullException(nameof(regionService));
        }

        /// <summary>
        /// Retrieve all regions
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<Region> GetRegionsAsync() => _regionService.GetAllAsync();

        /// <summary>
        /// Retrieve a region by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Region>> GetRegionByIdAsync(int id)
        {
            // Retrieve region
            Region region = await _regionService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (region == null)
                return NotFound();

            return region;
        }
    }
}
