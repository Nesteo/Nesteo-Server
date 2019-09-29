using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Nesteo.Server.Configuration
{
    public class JsonConfiguration : IConfigureOptions<JsonOptions>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JsonConfiguration(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        public void Configure(JsonOptions options)
        {
            // Indent JSON responses in development mode
            options.JsonSerializerOptions.WriteIndented = _webHostEnvironment.IsDevelopment();

            // Serialize enum values as strings (and the other way around)
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false));
        }
    }
}
