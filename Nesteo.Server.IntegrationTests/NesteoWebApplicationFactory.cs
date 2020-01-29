using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nesteo.Server.BackgroundTasks;
using Nesteo.Server.Data;

namespace Nesteo.Server.IntegrationTests
{
    public class NesteoWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, conf) => {
                // Use another appsettings file for these integration tests
                string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                conf.AddJsonFile(configFilePath, true);
            });

            builder.ConfigureServices(services => {
                using ServiceProvider serviceProvider = services.BuildServiceProvider();

                // Aquire a db context
                using IServiceScope scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<NesteoDbContext>();

                // Ensure the database is created
                dbContext.Database.Migrate();
            });
        }
    }
}
