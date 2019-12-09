using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nesteo.Server.Filters;
using Nesteo.Server.Models;
using Nesteo.Server.Options;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/nesting-boxes")]
    public class NestingBoxesController : ApiControllerBase
    {
        private readonly INestingBoxService _nestingBoxService;
        private readonly IInspectionService _inspectionService;
        private readonly IOptions<StorageOptions> _storageOptions;
        private readonly ILogger<NestingBoxesController> _logger;

        public NestingBoxesController(INestingBoxService nestingBoxService,
                                      IInspectionService inspectionService,
                                      IOptions<StorageOptions> storageOptions,
                                      ILogger<NestingBoxesController> logger)
        {
            _nestingBoxService = nestingBoxService ?? throw new ArgumentNullException(nameof(nestingBoxService));
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
            _storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        public async Task<IActionResult> EditNestingBoxAsync(string id, [FromBody] NestingBox nestingBox)
        {
            if (nestingBox.Id == null)
            {
                nestingBox.Id = id;
            }
            else if (nestingBox.Id != id)
            {
                ModelState.AddModelError("NestingBox.Id", "Nesting box ID is set but different from the resource URL.");
                return BadRequest(ModelState);
            }

            // Edit nesting box
            nestingBox = await _nestingBoxService.UpdateAsync(nestingBox, HttpContext.RequestAborted).ConfigureAwait(false);
            if (nestingBox == null)
                return Conflict();

            return NoContent();
        }

        /// <summary>
        /// Upload a new nesting box image
        /// </summary>
        /// <remarks>
        /// Replaces the old one, when existing.
        /// </remarks>
        /// <param name="id">Nesting box id</param>
        [HttpPost("{id}/upload-image")]
        [DisableFormValueModelBinding]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UploadNestingBoxImageAsync(string id)
        {
            if (!await _nestingBoxService.ExistsIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false))
                return NotFound();

            string imageFileName = await ReceiveMultipartImageFileUploadAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (imageFileName == null)
                return BadRequest(ModelState);

            await _nestingBoxService.SetImageFileNameAsync(id, imageFileName, HttpContext.RequestAborted).ConfigureAwait(false);

            return NoContent();
        }

        /// <summary>
        /// Download a nesting box image
        /// </summary>
        /// <param name="id">Nesting box id</param>
        [HttpGet("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNestingBoxImageAsync(string id)
        {
            string imageFileName = await _nestingBoxService.GetImageFileNameAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
            if (imageFileName == null)
                return NotFound();

            string imageFilePath = Path.Join(_storageOptions.Value.ImageUploadsDirectoryPath, imageFileName);
            var fileInfo = new FileInfo(imageFilePath);
            if (!fileInfo.Exists)
                return NotFound();

            string contentType = new FileExtensionContentTypeProvider().TryGetContentType(imageFilePath, out string result) ? result : "application/octet-stream";
            FileStream fileStream = fileInfo.OpenRead();

            return File(fileStream, contentType, true);
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

        /// <summary>
        /// Download nesting boxes csv
        /// </summary>
        [HttpGet("csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IAsyncEnumerable<string> ExportNestingBoxAsync()
        {
            return _nestingBoxService.ExportAllRowsAsync();
        }
    }
}
