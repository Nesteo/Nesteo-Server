using Nesteo.Server.Models;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class OwnersControllerTests : CrudControllerTestsBase<Owner>
    {
        public OwnersControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/owners") { }
    }
}
