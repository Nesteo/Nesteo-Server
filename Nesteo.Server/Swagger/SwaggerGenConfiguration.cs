using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nesteo.Server.Swagger.OperationFilters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nesteo.Server.Swagger
{
    public class SwaggerGenConfiguration : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1",
                               new OpenApiInfo {
                                   Title = "Nesteo API",
                                   Version = "v1",
                                   Description = "Server API of the Nesteo nesting box management application for ringing associations.",
                                   Contact = new OpenApiContact { Name = "Nesteo Team", Url = new Uri("https://github.com/Nesteo/") },
                                   License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://github.com/Nesteo/Nesteo-Server/blob/master/LICENSE") }
                               });

            // Tell the swagger generator where the comments file is stored
            string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            options.IncludeXmlComments(xmlPath);

            // Add operation filters
            options.OperationFilter<ResponseTypesOperationFilter>();

            // Add basic auth security definition
            options.AddSecurityDefinition("basic", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "basic" });

            // Apply basic auth to all API operations
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" } }, Array.Empty<string>() }
            });
        }
    }
}
