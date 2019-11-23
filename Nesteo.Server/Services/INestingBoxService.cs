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

        IAsyncEnumerable<string> GetAllTakenIdsWithPrefixAsync(string regionPrefix);

        IAsyncEnumerable<string> ExportAllRowsAsync();

        Task<NestingBox> AddAsync(NestingBox nestingBox, CancellationToken cancellationToken = default);

        Task<NestingBox> UpdateAsync(NestingBox nestingBox, CancellationToken cancellationToken = default);

        Task<NestingBox> SetImageFileNameAsync(string id, string imageFileName, CancellationToken cancellationToken = default);

        Task<string> GetImageFileNameAsync(string id, CancellationToken cancellationToken = default);
    }
}
