using System;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Nesteo.Server.Configuration
{
    public class ProblemDetailsConfiguration : IConfigureOptions<ProblemDetailsOptions>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProblemDetailsConfiguration(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Configure(ProblemDetailsOptions options)
        {
            // Provide richer exception information in development mode
            options.IncludeExceptionDetails = context => _webHostEnvironment.IsDevelopment();

            // Add request identifier to problem details responses
            options.OnBeforeWriteDetails = (context, problem) => problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

            // Add some status code mappings
            options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));
            options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));
            options.Map<DBConcurrencyException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status409Conflict));

            // Default mapping for all other exceptions
            options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
        }
    }
}
