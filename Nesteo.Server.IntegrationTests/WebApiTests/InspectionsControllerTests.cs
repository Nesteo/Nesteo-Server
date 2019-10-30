using System.Threading.Tasks;
using Nesteo.Server.Models;
using Xunit;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class InspectionsControllerTests : CrudControllerTestsBase<Inspection>
    {
        public InspectionsControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/inspections") { }

        [Fact]
        public Task GetAllPreviewsEndpointWorks() => TestModelCollectionEndpoint<InspectionPreview>($"{ControllerUrl}/previews");
    }
}
