using System.Threading.Tasks;
using Nesteo.Server.Models;
using Xunit;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class NestingBoxesControllerTests : CrudControllerTestsBase<NestingBox>
    {
        public NestingBoxesControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/nesting-boxes") { }

        [Fact]
        public Task GetAllPreviewsEndpointWorks() => TestModelCollectionEndpoint<NestingBoxPreview>($"{ControllerUrl}/previews");
    }
}
