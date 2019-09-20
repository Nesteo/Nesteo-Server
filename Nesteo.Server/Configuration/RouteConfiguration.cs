using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Nesteo.Server.Configuration
{
    public class RouteConfiguration:IConfigureOptions<RouteOptions>
    {
        public void Configure(RouteOptions options)
        {
            // Make URLs lowercase
            options.LowercaseUrls = true;
        }
    }
}
