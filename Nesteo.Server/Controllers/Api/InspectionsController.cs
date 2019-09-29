using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Data.Enums;
using Nesteo.Server.Models;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/inspections")]
    public class InspectionsController : ApiControllerBase
    {
        /// <summary>
        /// Retrieves all inspections
        /// </summary>
        // TODO: Use IAsyncEnumerable<> after EF Core upgrade
        [HttpGet]
        public Task<ActionResult<ICollection<Inspection>>> GetInspectionsAsync()
        {
            return Task.FromResult<ActionResult<ICollection<Inspection>>>(new List<Inspection> {
                new Inspection {
                    Id = 0,
                    NestingBox =
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
                        },
                    InspectionDate = new DateTime(2013, 12, 12, 12, 12, 12),
                    InspectedByUser = null,
                    HasBeenCleaned = false,
                    Condition = Condition.Good,
                    JustRepaired = false,
                    Occupied = true,
                    ContainsEggs = true,
                    EggCount = 0,
                    ChickCount = 5,
                    RingedChickCount = 4,
                    AgeInDays = 6,
                    FemaleParentBirdDiscovery = ParentBirdDiscovery.AlreadyRinged,
                    MaleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged,
                    Species = new Species { Id = 0, Name = "Dodo" },
                    ImageFileName = null,
                    Comment = "It has been a great inspection! It's true! Trust me! It has been the greatest inspection ever! It's true!",
                    LastUpdated = DateTime.UtcNow
                }
            });
        }

        /// <summary>
        /// Retrieves an inspection by id
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult<Inspection>> GetInspectionByIdAsync(int id)
        {
            if (id != 0)
                return Task.FromResult<ActionResult<Inspection>>(NotFound());

            return Task.FromResult<ActionResult<Inspection>>(new Inspection {
                Id = 0,
                NestingBox =
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
                    },
                InspectionDate = new DateTime(2013, 12, 12, 12, 12, 12),
                InspectedByUser = null,
                HasBeenCleaned = false,
                Condition = Condition.Good,
                JustRepaired = false,
                Occupied = true,
                ContainsEggs = true,
                EggCount = 0,
                ChickCount = 5,
                RingedChickCount = 4,
                AgeInDays = 6,
                FemaleParentBirdDiscovery = ParentBirdDiscovery.AlreadyRinged,
                MaleParentBirdDiscovery = ParentBirdDiscovery.NewlyRinged,
                Species = new Species { Id = 0, Name = "Dodo" },
                ImageFileName = null,
                Comment = "It has been a great inspection! It's true! Trust me! It has been the greatest inspection ever! It's true!",
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}
