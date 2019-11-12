using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface IInspectionService : ICrudService<Inspection, int?>
    {
        IAsyncEnumerable<InspectionPreview> GetAllPreviewsAsync();

        Task<InspectionPreview> FindPreviewByIdAsync(int id, CancellationToken cancellationToken = default);

        IAsyncEnumerable<Inspection> GetAllForNestingBoxIdAsync(string nestingBoxId);

        IAsyncEnumerable<InspectionPreview> GetAllPreviewsForNestingBoxIdAsync(string nestingBoxId);

        Task<Inspection> AddAsync(Inspection inspection, CancellationToken cancellationToken = default);

        Task<Inspection> UpdateAsync(Inspection inspection, CancellationToken cancellationToken = default);
    }
}
