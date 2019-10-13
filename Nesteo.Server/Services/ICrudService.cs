using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nesteo.Server.Services
{
    public interface ICrudService<TModel, TKey> where TModel : class
    {
        IAsyncEnumerable<TModel> GetAllAsync();

        Task<TModel> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
