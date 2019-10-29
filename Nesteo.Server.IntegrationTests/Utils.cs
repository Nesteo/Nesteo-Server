using System;
using System.Net.Http.Headers;
using System.Text;
using Nesteo.Server.BackgroundTasks;

namespace Nesteo.Server.IntegrationTests
{
    public static class Utils
    {
        public static AuthenticationHeaderValue CreateAuthenticationHeader(string userName, string password) =>
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")));

        public static AuthenticationHeaderValue CreateDefaultAuthenticationHeader() =>
            CreateAuthenticationHeader(CreateDefaultUserTask.DefaultUserName, CreateDefaultUserTask.DefaultPassword);
    }
}
