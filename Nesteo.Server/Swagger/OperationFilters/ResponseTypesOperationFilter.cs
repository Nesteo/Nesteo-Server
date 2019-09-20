using System.Collections.Generic;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nesteo.Server.Swagger.OperationFilters
{
    public class ResponseTypesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Create response schemas
            OpenApiSchema validationProblemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(ValidationProblemDetails), context.SchemaRepository);
            // TODO: Try again when https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1202 is fixed
            // OpenApiSchema exceptionProblemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(ExceptionProblemDetails), context.SchemaRepository);
            OpenApiSchema exceptionProblemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);
            OpenApiSchema statusCodeProblemDetailsSchema = context.SchemaGenerator.GenerateSchema(typeof(StatusCodeProblemDetails), context.SchemaRepository);

            // Add possible operation responses
            operation.Responses.TryAdd(StatusCodes.Status400BadRequest.ToString(),
                                       new OpenApiResponse {
                                           Description = "Bad Request",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = validationProblemDetailsSchema } }
                                           }
                                       });
            operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(),
                                       new OpenApiResponse {
                                           Description = "Unauthorized",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = statusCodeProblemDetailsSchema } }
                                           }
                                       });
            operation.Responses.TryAdd(StatusCodes.Status403Forbidden.ToString(),
                                       new OpenApiResponse {
                                           Description = "Forbidden",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = statusCodeProblemDetailsSchema } }
                                           }
                                       });
            operation.Responses.TryAdd(StatusCodes.Status404NotFound.ToString(),
                                       new OpenApiResponse {
                                           Description = "NotFound",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = statusCodeProblemDetailsSchema } }
                                           }
                                       });
            operation.Responses.TryAdd(StatusCodes.Status409Conflict.ToString(),
                                       new OpenApiResponse {
                                           Description = "Conflict",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = statusCodeProblemDetailsSchema } }
                                           }
                                       });
            operation.Responses.TryAdd(StatusCodes.Status500InternalServerError.ToString(),
                                       new OpenApiResponse {
                                           Description = "Internal Server Error",
                                           Content = new Dictionary<string, OpenApiMediaType> {
                                               { "application/json", new OpenApiMediaType { Schema = exceptionProblemDetailsSchema } }
                                           }
                                       });
        }
    }
}
