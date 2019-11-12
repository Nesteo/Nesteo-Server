using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Enums;
using Nesteo.Server.IdGeneration;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/nesting-boxes")]
    public class NestingBoxesController : ApiControllerBase
    {
        private readonly INestingBoxService _nestingBoxService;
        private readonly IInspectionService _inspectionService;
        private readonly INestingBoxIdGenerator _nestingBoxIdGenerator;
        private readonly IUserService _userService;

        public NestingBoxesController(INestingBoxService nestingBoxService,
                                      IInspectionService inspectionService,
                                      INestingBoxIdGenerator nestingBoxIdGenerator,
                                      IUserService userService)
        {
            _nestingBoxService = nestingBoxService ?? throw new ArgumentNullException(nameof(nestingBoxService));
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
            _nestingBoxIdGenerator = nestingBoxIdGenerator ?? throw new ArgumentNullException(nameof(nestingBoxIdGenerator));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
        /// Create a new nesting box
        /// </summary>
        /// <remarks>
        /// In case no ID is specified, a new one will be calculated automatically. You can get the generated ID from the response.
        /// Other entries included in this nesting box entry will be automatically created or updated as needed.
        /// </remarks>
        /// <param name="nestingBox">The nesting box to create</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NestingBox>> CreateNestingBoxAsync([FromBody] NestingBox nestingBox)
        {
            // Create nesting box
            nestingBox = await _nestingBoxService.AddAsync(nestingBox, HttpContext.RequestAborted).ConfigureAwait(false);
            if (nestingBox == null)
                return Conflict();

            return CreatedAtAction(nameof(GetNestingBoxByIdAsync), new { id = nestingBox.Id }, nestingBox);
        }

        /// <summary>
        /// Edit an existing nesting box
        /// </summary>
        /// <param name="id">Nesting box id</param>
        /// <param name="nestingBox">The modified nesting box</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> EditNestingBoxAsync(string id, [FromBody] NestingBox nestingBox)
        {
            if (nestingBox.Id == null)
                nestingBox.Id = id;
            else if (nestingBox.Id != id)
                return BadRequest();

            // Edit nesting box
            nestingBox = await _nestingBoxService.UpdateAsync(nestingBox, HttpContext.RequestAborted).ConfigureAwait(false);
            if (nestingBox == null)
                return Conflict();

            return NoContent();
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
            return _inspectionService.GetAllForNestingBoxIdAsync(id);
        }

        /// <summary>
        /// Return all inspection previews for a nesting box
        /// </summary>
        [HttpGet("{id}/inspections/previews")]
        public IAsyncEnumerable<InspectionPreview> GetInspectionPreviewsByNestingBoxIdAsync(string id)
        {
            return _inspectionService.GetAllPreviewsForNestingBoxIdAsync(id);
        }
    }
}
