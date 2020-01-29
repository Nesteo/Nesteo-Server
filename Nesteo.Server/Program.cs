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

            // Prepare host
            await PrepareHostAsync(host).ConfigureAwait(false);

            // Run application
            await host.RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => {
                webBuilder.ConfigureKestrel(options => {
                    options.Limits.MaxRequestBodySize = null;
                });
                webBuilder.UseStartup<Startup>();
            });

        public static Task PrepareHostAsync(IHost host)
        {
            // Validate and prepare the AutoMapper configuration
            var mapperConfigurationProvider = host.Services.GetRequiredService<IConfigurationProvider>();
#if DEBUG
            mapperConfigurationProvider.AssertConfigurationIsValid();
#endif
            mapperConfigurationProvider.CompileMappings();

            return Task.CompletedTask;
        }
    }
}
