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

        public NestingBoxesController(INestingBoxService nestingBoxService)
        {
            _nestingBoxService = nestingBoxService ?? throw new ArgumentNullException(nameof(nestingBoxService));
        }

        /// <summary>
        /// Retrieves all nesting boxes
        /// </summary>
        [HttpGet]
        public IAsyncEnumerable<NestingBox> GetNestingBoxesAsync()
        {
            return _nestingBoxService.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a nesting box by id
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
        /// Previews all nesting boxes with a reduced set of data
        /// </summary>
        [HttpGet("previews")]
        public IAsyncEnumerable<NestingBoxPreview> GetNestingBoxPreviewsAsync()
        {
            return _nestingBoxService.GetAllPreviewsAsync();
        }

        /// <summary>
        /// Previews a nesting box by id with a reduced set of data
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
    }
}
