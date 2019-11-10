using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Enums;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/inspections")]
    public class InspectionsController : ApiControllerBase
    {
        private readonly IInspectionService _inspectionService;

        public InspectionsController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
        }

        /// <summary>
        /// Retrieve all inspections
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<Inspection> GetInspectionsAsync()
        {
            return _inspectionService.GetAllAsync();
        }

        /// <summary>
        /// Retrieve an inspection by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Inspection>> GetInspectionByIdAsync(int id)
        {
            // Retrieve inspection
            Inspection inspection = await _inspectionService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (inspection == null)
                return NotFound();

            return inspection;
        }

        /// <summary>
        /// Create a new inspection
        /// </summary>
        /// <remarks>
        /// No ID should be set, because it will get generated automatically.
        /// </remarks>
        /// <param name="inspection">The inspection to create</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Inspection>> CreateInspectionAsync([FromBody] Inspection inspection)
        {
            // Create inspection
            inspection = await _inspectionService.AddAsync(inspection, HttpContext.RequestAborted).ConfigureAwait(false);
            if (inspection == null)
                return Conflict();

            return CreatedAtAction(nameof(GetInspectionByIdAsync), new { id = inspection.Id }, inspection);
        }

        /// <summary>
        /// Edit an existing inspection
        /// </summary>
        /// <param name="id">Inspection id</param>
        /// <param name="inspection">The modified inspection</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> EditInspectionAsync(int id, [FromBody] Inspection inspection)
        {
            if (inspection.Id == null)
                inspection.Id = id;
            else if (inspection.Id != id)
                return BadRequest();

            // Edit inspection
            inspection = await _inspectionService.UpdateAsync(inspection, HttpContext.RequestAborted).ConfigureAwait(false);
            if (inspection == null)
                return Conflict();

            return NoContent();
        }

        /// <summary>
        /// Preview all inspections with a reduced set of data
        /// </summary>
        [HttpGet("previews")]
        public IAsyncEnumerable<InspectionPreview> GetInspectionPreviewsAsync()
        {
            return _inspectionService.GetAllPreviewsAsync();
        }

        /// <summary>
        /// Preview an inspection by id with a reduced set of data
        /// </summary>
        [HttpGet("previews/{id}")]
        public async Task<ActionResult<InspectionPreview>> GetInspectionPreviewByIdAsync(int id)
        {
            // Retrieve inspection preview
            InspectionPreview inspectionPreview = await _inspectionService.FindPreviewByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (inspectionPreview == null)
                return NotFound();

            return inspectionPreview;
        }
    }
}
