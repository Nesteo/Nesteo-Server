using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        /// <summary>
        /// Return all inspections for a nesting box
        /// </summary>
        [HttpGet("inspectionByBox/{id}")]
        public IAsyncEnumerable<Inspection> etInspectionsByBoxIdAsync(string id)
        {
            return  _inspectionService.FindByBoxIdAsync(id);
        }
    }
}
