using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Nesteo.Server.BackgroundTasks;
using Nesteo.Server.Models;
using Xunit;

namespace Nesteo.Server.IntegrationTests.WebApiTests
{
    public abstract class CrudControllerTestsBase<TModel> : IClassFixture<NesteoWebApplicationFactory<Startup>> where TModel : class
    {
        protected NesteoWebApplicationFactory<Startup> WebApplicationFactory { get; }

        protected string ControllerUrl { get; }

        protected CrudControllerTestsBase(NesteoWebApplicationFactory<Startup> webApplicationFactory, string controllerUrl)
        {
            WebApplicationFactory = webApplicationFactory ?? throw new ArgumentNullException(nameof(webApplicationFactory));
            ControllerUrl = controllerUrl ?? throw new ArgumentNullException(nameof(controllerUrl));
        }

        [Fact]
        public async Task RequiresAuthentication()
        {
            using HttpClient client = WebApplicationFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(ControllerUrl).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public Task GetAllEndpointWorks() => TestModelCollectionEndpoint<TModel>(ControllerUrl);

        protected async Task TestModelCollectionEndpoint<TItem>(string url) where TItem : class
        {
            using HttpClient client = WebApplicationFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = Utils.CreateDefaultAuthenticationHeader();

            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            ICollection<TItem> collection = await response.Content.ReadAsAsync<ICollection<TItem>>().ConfigureAwait(false);
            Assert.All(collection, Assert.NotNull);
        }
    }
}
