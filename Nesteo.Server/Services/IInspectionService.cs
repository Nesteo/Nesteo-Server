using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface IInspectionService: ICrudService<Inspection, int>
    {
        IAsyncEnumerable<InspectionPreview> GetAllPreviewsAsync();

        Task<InspectionPreview> FindPreviewByIdAsync(int id, CancellationToken cancellationToken = default);

        IAsyncEnumerable<Inspection> GetAllForNestingBoxIdAsync(string nestingBoxId);
    }
}
