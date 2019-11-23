using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nesteo.Server.Options;
using Nesteo.Server.Utils;

namespace Nesteo.Server.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class ApiControllerBase : Controller
    {
        private static readonly string[] ValidImageFileExtensions = { ".jpg", ".png" };

        protected async Task<string> ReceiveMultipartImageFileUploadAsync(string namePrefix, CancellationToken cancellationToken = default)
        {
            IOptions<StorageOptions> storageOptions = HttpContext.RequestServices.GetRequiredService<IOptions<StorageOptions>>();
            ILogger<ApiControllerBase> logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApiControllerBase>>();

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", "Multipart request expected.");
                return null;
            }

            string boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
            var multipartReader = new MultipartReader(boundary, HttpContext.Request.Body);

            MultipartSection section = await multipartReader.ReadNextSectionAsync(cancellationToken).ConfigureAwait(false);
            if (section == null)
            {
                ModelState.AddModelError("File", "No multipart section found.");
                return null;
            }

            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition))
            {
                ModelState.AddModelError("File", "Content disposition header expected.");
                return null;
            }

            if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
            {
                ModelState.AddModelError("File", "File content disposition expected.");
                return null;
            }

            string untrustedFileName = contentDisposition.FileName.Value;
            string fileExtension = Path.GetExtension(untrustedFileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !ValidImageFileExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", $"File extension {fileExtension} is not allowed. Valid extensions are: {string.Join(", ", ValidImageFileExtensions)}");
                return null;
            }

            Directory.CreateDirectory(storageOptions.Value.ImageUploadsDirectoryPath);

            string targetFileName = $"{namePrefix}-{Guid.NewGuid()}{fileExtension}";
            string targetFilePath = Path.Join(storageOptions.Value.ImageUploadsDirectoryPath, targetFileName);

            try
            {
                await using FileStream targetStream = System.IO.File.Create(targetFilePath);
                await section.Body.CopyToAsync(targetStream, HttpContext.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception)
            {
                System.IO.File.Delete(targetFilePath);
                throw;
            }

            logger.LogInformation($"Successfully uploaded file {untrustedFileName} to {targetFilePath}");

            return targetFileName;
        }
    }
}
