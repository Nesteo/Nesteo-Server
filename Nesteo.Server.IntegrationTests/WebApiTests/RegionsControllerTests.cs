using Nesteo.Server.Models;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class RegionsControllerTests : CrudControllerTestsBase<Region>
    {
        public RegionsControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/regions") { }
    }
}
