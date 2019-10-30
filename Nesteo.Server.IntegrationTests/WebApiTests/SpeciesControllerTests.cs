using Nesteo.Server.Models;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class SpeciesControllerTests : CrudControllerTestsBase<Species>
    {
        public SpeciesControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/species") { }
    }
}
