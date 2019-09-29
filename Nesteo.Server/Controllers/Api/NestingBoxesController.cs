using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Data.Enums;
using Nesteo.Server.Models;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/nesting-boxes")]
    public class NestingBoxesController : ApiControllerBase
    {
        /// <summary>
        /// Retrieves all nesting boxes
        /// </summary>
        // TODO: Use IAsyncEnumerable<> after EF Core upgrade
        [HttpGet]
        public Task<ActionResult<ICollection<NestingBox>>> GetNestingBoxesAsync()
        {
            return Task.FromResult<ActionResult<ICollection<NestingBox>>>(new List<NestingBox> {
                new NestingBox {
                    Id = "F000001",
                    Region = new Region { Id = 0, Name = "The only forest in germany", NestingBoxIdPrefix = "F" },
                    OldId = null,
                    ForeignId = "x234362",
                    CoordinateLongitude = -97.142212,
                    CoordinateLatitude = 30.081692,
                    HangUpDate = new DateTime(2012, 12, 12, 12, 12, 12),
                    HangUpUser = null,
                    Owner = new Owner { Id = 0, Name = "He-who-must-not-be-named" },
                    Material = Material.TreatedWood,
                    HoleSize = HoleSize.Large,
                    ImageFileName = null,
                    Comment = "This is a test",
                    LastUpdated = DateTime.UtcNow
                }
            });
        }

        /// <summary>
        /// Retrieves a nesting box by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult<NestingBox>> GetNestingBoxByIdAsync(string id)
        {
            if (id != "F000001")
                return Task.FromResult<ActionResult<NestingBox>>(NotFound());

            return Task.FromResult<ActionResult<NestingBox>>(new NestingBox {
                Id = "F000001",
                Region = new Region { Id = 0, Name = "The only forest in germany", NestingBoxIdPrefix = "F" },
                OldId = null,
                ForeignId = "x234362",
                CoordinateLongitude = -97.142212,
                CoordinateLatitude = 30.081692,
                HangUpDate = new DateTime(2012, 12, 12, 12, 12, 12),
                HangUpUser = null,
                Owner = new Owner { Id = 0, Name = "He-who-must-not-be-named" },
                Material = Material.TreatedWood,
                HoleSize = HoleSize.Large,
                ImageFileName = null,
                Comment = "This is a test",
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}
