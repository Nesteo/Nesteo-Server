using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;

namespace Nesteo.Server.Services.Implementations
{
    public abstract class CrudServiceBase<TEntity, TModel, TKey> : ICrudService<TModel, TKey> where TEntity : class, IEntity<TKey> where TModel : class
    {
        protected NesteoDbContext DbContext { get; }

        protected IMapper Mapper { get; }

        /// <summary>
        /// The database set for the CRUD operations
        /// </summary>
        protected abstract DbSet<TEntity> Entities { get; }

        protected CrudServiceBase(NesteoDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IAsyncEnumerable<TModel> GetAllAsync()
        {
            // Map all entries to the model type and retrieve them as async stream
            return Entities.ProjectTo<TModel>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<TModel> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // Search for the entry with the given ID and map it to the model type
            return Entities.Where(entity => entity.Id.Equals(id)).ProjectTo<TModel>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        // TODO: Add remaining CRUD operations
    }
}
