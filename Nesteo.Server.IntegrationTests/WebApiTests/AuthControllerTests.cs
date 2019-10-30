using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Nesteo.Server.BackgroundTasks;
using Nesteo.Server.Models;
using Xunit;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public class AuthControllerTests : IClassFixture<NesteoWebApplicationFactory<Startup>>
    {
        private readonly NesteoWebApplicationFactory<Startup> _webApplicationFactory;

        public AuthControllerTests(NesteoWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory ?? throw new ArgumentNullException(nameof(webApplicationFactory));
        }

        [Fact]
        public async Task RequiresAuthentication()
        {
            using HttpClient client = _webApplicationFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("api/v1/auth").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ReturnsCorrectAuthInformation()
        {
            using HttpClient client = _webApplicationFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = Utils.CreateDefaultAuthenticationHeader();

            HttpResponseMessage response = await client.GetAsync("api/v1/auth").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            User user = await response.Content.ReadAsAsync<User>().ConfigureAwait(false);
            Assert.Equal(CreateDefaultUserTask.DefaultFirstName, user.FirstName);
            Assert.Equal(CreateDefaultUserTask.DefaultLastName, user.LastName);
            Assert.Equal(CreateDefaultUserTask.DefaultUserName, user.UserName);
        }
    }
}
