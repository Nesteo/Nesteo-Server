using System;
using System.Linq;
using System.Threading.Tasks;
using Nesteo.Server.Controllers;
using Nesteo.Server.Services;
using Moq;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class NestingBoxControllerTests
    {
        private readonly INestingBoxService _nestingBoxService;

        public NestingBoxControllerTests(INestingBoxService nestingBoxService)
        {
            _nestingBoxService = nestingBoxService ?? throw new ArgumentNullException(nameof(nestingBoxService));
        }

        [Fact]
        public void ThrowsWhenServerProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => _nestingBoxService);
        }
    }
}
