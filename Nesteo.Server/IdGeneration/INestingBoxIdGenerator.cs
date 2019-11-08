using System.Collections.Generic;
using System.Threading;
using Nesteo.Server.Models;

namespace Nesteo.Server.IdGeneration
{
    public interface INestingBoxIdGenerator
    {
        IAsyncEnumerable<string> GetNextIdsAsync(User user, Region region, int count, CancellationToken cancellationToken = default);
    }
}
