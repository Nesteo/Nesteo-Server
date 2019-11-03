using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface INestingBoxService : ICrudService<NestingBox, string>
    {
        IAsyncEnumerable<NestingBoxPreview> GetAllPreviewsAsync();

        Task<NestingBoxPreview> FindPreviewByIdAsync(string id, CancellationToken cancellationToken = default);

        IAsyncEnumerable<string> GetAllTakenIdsAsync();
    }
}
