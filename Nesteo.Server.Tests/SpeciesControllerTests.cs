using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class SpeciesControllerTests
    {
        private readonly ISpeciesService _speciesService;

        public SpeciesControllerTests(ISpeciesService speciesService)
        {
            _speciesService = speciesService ?? throw new ArgumentNullException(nameof(speciesService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _speciesService);
        }
    }
}
