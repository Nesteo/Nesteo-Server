using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class UsersControllerTests
    {
        private readonly IUserService _userService;

        public UsersControllerTests(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _userService);
        }
    }
}
