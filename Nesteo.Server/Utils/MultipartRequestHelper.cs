using System;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace Nesteo.Server.Utils
{
    // Source: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.0#upload-large-files-with-streaming
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType)
        {
            const int lengthLimit = 70;

            string boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
                throw new InvalidDataException("Missing content-type boundary.");

            if (boundary.Length > lengthLimit)
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");

            return boundary;
        }

        public static bool IsMultipartContentType(string contentType)
            => !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;

        // Content-Disposition: form-data; name="key";
        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
            => contentDisposition != null && contentDisposition.DispositionType.Equals("form-data") && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);

        // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
            => contentDisposition != null && contentDisposition.DispositionType.Equals("form-data") && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
    }
}
