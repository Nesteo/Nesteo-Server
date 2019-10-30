using Nesteo.Server.Models;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class UsersControllerTests : CrudControllerTestsBase<User>
    {
        public UsersControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory, "api/v1/users") { }
    }
}
