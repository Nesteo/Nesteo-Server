using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class InspectionsControllerTests
    {
        private readonly IInspectionService _inspectionService;

        public InspectionsControllerTests(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService ?? throw new ArgumentNullException(nameof(inspectionService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _inspectionService);
        }
    }
}
