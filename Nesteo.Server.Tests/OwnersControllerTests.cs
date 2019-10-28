using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class OwnersControllerTests
    {
        private readonly IOwnerService _ownerService;

        public OwnersControllerTests(IOwnerService ownerService)
        {
            _ownerService = ownerService ?? throw new ArgumentNullException(nameof(ownerService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ownerService);
        }
    }
}
