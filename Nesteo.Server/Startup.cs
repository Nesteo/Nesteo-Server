using System;
using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using MarcusW.SharpUtils.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nesteo.Server.Authentication.Configuration;
using Nesteo.Server.Authorization;
using Nesteo.Server.BackgroundTasks;
using Nesteo.Server.Configuration;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Identity;
using Nesteo.Server.Services;
using Nesteo.Server.Services.Implementations;
using Nesteo.Server.Swagger;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Nesteo.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // This method gets called by the runtime and configures the dependency injection container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Make accessing the http context using dependency injection possible
            services.AddHttpContextAccessor();

            // Add database access
            services.AddDbContextPool<NesteoDbContext>(options => {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                                 mySqlOptions => {
                                     mySqlOptions.ServerVersion(new Version(10, 3), ServerType.MariaDb);
                                 });
            });

            // Add itentity system
            services.AddIdentityCore<NesteoUser>().AddRoles<NesteoRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<NesteoDbContext>();
            services.AddScoped<SignInManager<NesteoUser>>();

            // Add authentication
            AuthenticationBuilder authenticationBuilder = services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            });

            // Add cookie for the identity system authentication
            authenticationBuilder.AddApplicationCookie();

            // Add basic auth for the API endpoints
            authenticationBuilder.AddBasicAuthentication();

            // Configure authentication
            services.ConfigureOptions<CookieAuthenticationConfiguration>();
            services.ConfigureOptions<BasicAuthenticationConfiguration>();

            // Add authorization
            services.AddAuthorization(options => {
                // Configure authorization policies
                options.AddPolicy(PolicyNames.ApiUserSignedIn,
                                  policy => policy.AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser());
                options.AddPolicy(PolicyNames.ManagementInterfaceUserSignedIn,
                                  policy => policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme).RequireAuthenticatedUser());
            });

            // Add response compression
            services.AddResponseCompression();

            // Add problem details
            services.AddProblemDetails();

            // Add support for API controllers
            services.AddControllers();

            // Add support for razor pages
            services.AddRazorPages();

            // Add health checks
            services.AddHealthChecks().AddDbContextCheck<NesteoDbContext>();

            // Add swagger api documentation
            services.AddSwaggerGen();
            services.ConfigureOptions<SwaggerGenConfiguration>();

            // Add background tasks that ensures, that a login on fresh installations is possible
            services.AddHostedService<CreateDefaultUserTask>();

            // Add some server configuration
            services.ConfigureOptions<IdentityConfiguration>();
            services.ConfigureOptions<ProblemDetailsConfiguration>();
            services.ConfigureOptions<RouteConfiguration>();
            services.ConfigureOptions<JsonConfiguration>();

            // Add mapping
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add support for late dependencies
            services.AddLateDependenciesSupport();

            // Add service implementations
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime and configures the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Error handling for API requests
            app.UseWhen(IsApiRequest,
                        appBuilder => {
                            // Return RFC 7807 problem details
                            appBuilder.UseProblemDetails();
                        });

            // Error handling for all other requests
            app.UseWhen(context => !IsApiRequest(context),
                        appBuilder => {
                            // Differentiate by execution environment
                            if (env.IsDevelopment())
                            {
                                // Configure development error handling
                                appBuilder.UseDeveloperExceptionPage();
                                appBuilder.UseDatabaseErrorPage();
                            }
                            else
                            {
                                // Redirect to error page on errors
                                appBuilder.UseExceptionHandler("/Error");
                            }

                            // Status code pages
                            appBuilder.UseStatusCodePages();
                        });

            // Compress responses
            app.UseResponseCompression();

            // Serve static files
            app.UseStaticFiles();

            // Serve swagger API documentation
            app.UseSwagger();

            // Serve swagger UI API testing tool
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nesteo API v1");
                options.DocumentTitle = "Nesteo API Documentation";
            });

            // Serve swagger API documentation page
            app.UseReDoc(options => {
                options.SpecUrl = "/swagger/v1/swagger.json";
                options.DocumentTitle = "Nesteo API Documentation";
                options.ExpandResponses("200");
            });

            // Use routing
            app.UseRouting();

            // Use authentication
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                // Health check endpoint
                endpoints.MapHealthChecks("/health");

                // Endpoints for the API
                endpoints.MapControllers().RequireAuthorization(PolicyNames.ApiUserSignedIn);

                // Endpoints for razor pages
                endpoints.MapRazorPages().RequireAuthorization(PolicyNames.ManagementInterfaceUserSignedIn);
            });
        }

        private static bool IsApiRequest(HttpContext httpContext) => httpContext.Request.Path.StartsWithSegments("/api");
    }
}
