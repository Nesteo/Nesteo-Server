using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Models;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/regions")]
    public class RegionsController : ApiControllerBase
    {
        /// <summary>
        /// Retrieves all regions
        /// </summary>
        // TODO: Use IAsyncEnumerable<> after EF Core upgrade
        [HttpGet]
        public Task<ActionResult<ICollection<Region>>> GetRegionsAsync()
        {
            return Task.FromResult<ActionResult<ICollection<Region>>>(new List<Region> { new Region { Id = 0, Name = "The only forest in germany", NestingBoxIdPrefix = "F" } });
        }

        /// <summary>
        /// Retrieves a region by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult<Region>> GetRegionByIdAsync(int id)
        {
            if (id != 0)
                return Task.FromResult<ActionResult<Region>>(NotFound());

            return Task.FromResult<ActionResult<Region>>(new Region { Id = 0, Name = "The only forest in germany", NestingBoxIdPrefix = "F" });
        }
    }
}
