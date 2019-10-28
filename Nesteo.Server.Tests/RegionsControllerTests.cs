using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class RegionsControllerTests
    {
        private readonly IRegionService _regionService;

        public RegionsControllerTests(IRegionService regionService)
        {
            _regionService = regionService ?? throw new ArgumentNullException(nameof(regionService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _regionService);
        }
    }
}
