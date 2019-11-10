using System.Collections.Generic;
using System.Threading;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.IdGeneration
{
    public interface INestingBoxIdGenerator
    {
        IAsyncEnumerable<string> GetNextIdsAsync(INestingBoxService nestingBoxService, Region region, int count, CancellationToken cancellationToken = default);
    }
}
