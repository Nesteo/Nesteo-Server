using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Nesteo.Server.Filters;
using Nesteo.Server.Models;
using Nesteo.Server.Options;
using Nesteo.Server.Services;

namespace Nesteo.Server.Controllers.Api
{
    [Route("api/v1/inspections")]
    public class InspectionsController : ApiControllerBase
    {
        private readonly IInspectionService _inspectionService;
        private readonly IOptions<StorageOptions> _storageOptions;

        public InspectionsController(IInspectionService inspectionService, IOptions<StorageOptions> storageOptions)
        {
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
            _storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
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
        public async Task<IActionResult> EditInspectionAsync(int id, [FromBody] Inspection inspection)
        {
            if (inspection.Id == null)
            {
                inspection.Id = id;
            }
            else if (inspection.Id != id)
            {
                ModelState.AddModelError("Inspection.Id", "Inspection ID is set but different from the resource URL.");
                return BadRequest(ModelState);
            }

            // Edit inspection
            inspection = await _inspectionService.UpdateAsync(inspection, HttpContext.RequestAborted).ConfigureAwait(false);
            if (inspection == null)
                return Conflict();

            return NoContent();
        }

        /// <summary>
        /// Upload a new inspection image
        /// </summary>
        /// <remarks>
        /// Replaces the old one, when existing.
        /// </remarks>
        /// <param name="id">Inspection id</param>
        [HttpPost("{id}/upload-image")]
        [DisableFormValueModelBinding]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UploadInspectionImageAsync(int id)
        {
            if (!await _inspectionService.ExistsIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false))
                return NotFound();

            string imageFileName = await ReceiveMultipartImageFileUploadAsync(id.ToString(), HttpContext.RequestAborted).ConfigureAwait(false);
            if (imageFileName == null)
                return BadRequest(ModelState);

            await _inspectionService.SetImageFileNameAsync(id, imageFileName, HttpContext.RequestAborted).ConfigureAwait(false);

            return NoContent();
        }

        /// <summary>
        /// Download an inspection image
        /// </summary>
        /// <param name="id">Inspection id</param>
        [HttpGet("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInspectionImageAsync(int id)
        {
            string imageFileName = await _inspectionService.GetImageFileNameAsync(id, HttpContext.RequestAborted).ConfigureAwait(false);
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
        /// Download inspections csv
        /// </summary>
        [HttpGet("csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IAsyncEnumerable<string> ExportInspectionsAsync()
        {
            return _inspectionService.ExportAllRowsAsync();
        }
    }
}
