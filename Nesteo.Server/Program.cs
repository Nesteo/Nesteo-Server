using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nesteo.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            // Validate and prepare the AutoMapper configuration
            IConfigurationProvider mapperConfigurationProvider = host.Services.GetRequiredService<IConfigurationProvider>();
#if DEBUG
            mapperConfigurationProvider.AssertConfigurationIsValid();
#endif
            mapperConfigurationProvider.CompileMappings();

            // Run application
            await host.RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        // TODO: Required for the Entity Framework Core 2.X migration tool. This should be removed as soon as we can upgrade to EF core 3.0
        // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/797
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
