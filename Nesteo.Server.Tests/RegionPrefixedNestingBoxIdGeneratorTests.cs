using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nesteo.Server.IdGeneration;
using Nesteo.Server.Models;
using Nesteo.Server.Services;
using Xunit;

namespace Nesteo.Server.Tests
{
    public class RegionPrefixedNestingBoxIdGeneratorTests
    {
        [Theory]
        [InlineData("0", 1, new string[] { }, new[] { "000000" })]
        [InlineData("A", 3, new[] { "A00001" }, new[] { "A00000", "A00002", "A00003" })]
        [InlineData("A", 3, new[] { "A00000", "A00001", "A00002" }, new[] { "A00003", "A00004", "A00005" })]
        public async Task ReturnsCorrectIds(string prefix, int count, string[] takenIds, string[] expectedNextIds)
        {
            var nestingBoxServiceMock = new Mock<INestingBoxService>(MockBehavior.Strict);
            nestingBoxServiceMock.Setup(service => service.GetAllTakenIdsWithPrefixAsync(prefix)).Returns(takenIds.ToAsyncEnumerable);

            var generator = new RegionPrefixedNestingBoxIdGenerator();

            List<string> ids = await generator.GetNextIdsAsync(nestingBoxServiceMock.Object, new Region { Id = 0, Name = "Test Region", NestingBoxIdPrefix = prefix }, count).ToListAsync()
                                              .ConfigureAwait(false);
            Assert.Equal(expectedNextIds, ids);
        }
    }
}
