using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Nesteo.Server.Result
{
    public class CsvFileResult : IActionResult
    {
        private readonly IAsyncEnumerable<string> _records;
        private readonly string _fileDownloadName;

        public CsvFileResult(IAsyncEnumerable<string> records, string fileDownloadName)
        {
            _records = records ?? throw new ArgumentNullException(nameof(records));
            _fileDownloadName = fileDownloadName ?? throw new ArgumentNullException(nameof(fileDownloadName));
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;

            // Set response content type
            response.ContentType = "text/csv";

            // Set content disposition to make this a download
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(_fileDownloadName);
            response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();

            try
            {
                // Write CSV records to response stream
                await using var streamWriter = new StreamWriter(response.Body, Encoding.UTF8, -1, true);
                await foreach (string record in _records)
                    await streamWriter.WriteLineAsync(record);
            }
            catch (OperationCanceledException)
            {
                // Don't throw this exception, it's most likely caused by the client disconnecting.
                // However, if it was cancelled for any other reason we need to prevent empty responses.
                context.HttpContext.Abort();
            }
        }
    }
}
