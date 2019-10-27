using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Data.Enums;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/nesting-boxes")]
    public class NestingBoxesController : ApiControllerBase
    {
        private readonly INestingBoxService _nestingBoxService;
        private readonly IInspectionService _inspectionService;

        public NestingBoxesController(INestingBoxService nestingBoxService, IInspectionService inspectionService)
        {
            _nestingBoxService = nestingBoxService ?? throw new ArgumentNullException(nameof(nestingBoxService));
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
        }

        /// <summary>
        /// Retrieve all nesting boxes
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<NestingBox> GetNestingBoxesAsync()
        {
            return _nestingBoxService.GetAllAsync();
        }

        /// <summary>
        /// Retrieve a nesting box by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NestingBox>> GetNestingBoxByIdAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // Retrieve nesting box
            NestingBox nestingBox = await _nestingBoxService.FindByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (nestingBox == null)
                return NotFound();

            return nestingBox;
        }

        /// <summary>
        /// Preview all nesting boxes with a reduced set of data
        /// </summary>
        [HttpGet("previews")]
        public IAsyncEnumerable<NestingBoxPreview> GetNestingBoxPreviewsAsync()
        {
            return _nestingBoxService.GetAllPreviewsAsync();
        }

        /// <summary>
        /// Preview a nesting box by id with a reduced set of data
        /// </summary>
        [HttpGet("previews/{id}")]
        public async Task<ActionResult<NestingBoxPreview>> GetNestingBoxPreviewByIdAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // Retrieve nesting box preview
            NestingBoxPreview nestingBoxPreview = await _nestingBoxService.FindPreviewByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (nestingBoxPreview == null)
                return NotFound();

            return nestingBoxPreview;
        }

        /// <summary>
        /// Return all inspections for a nesting box
        /// </summary>
        [HttpGet("{id}/inspections")]
        public IAsyncEnumerable<Inspection> GetInspectionsByNestingBoxIdAsync(string id)
        {
            return  _inspectionService.FindByNestingBoxIdAsync(id);
        }
    }
}
