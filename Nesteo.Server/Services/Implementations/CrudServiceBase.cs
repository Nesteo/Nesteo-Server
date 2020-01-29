using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;

namespace Nesteo.Server.Services.Implementations
{
    public abstract class CrudServiceBase<TEntity, TModel, TKey> : ICrudService<TModel, TKey> where TEntity : class, IEntity<TKey> where TModel : class
    {
        protected NesteoDbContext DbContext { get; }

        protected IMapper Mapper { get; }

        protected DbSet<TEntity> Entities { get; }

        protected CrudServiceBase(NesteoDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Entities = dbContext.Set<TEntity>();
        }

        public virtual IAsyncEnumerable<TModel> GetAllAsync()
        {
            // Map all entries to the model type and retrieve them as async stream
            return Entities.AsNoTracking().OrderBy(entity => entity.Id).ProjectTo<TModel>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public virtual Task<TModel> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // Search for the entry with the given ID and map it to the model type
            return Entities.AsNoTracking().Where(entity => entity.Id.Equals(id)).ProjectTo<TModel>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task<bool> ExistsIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return Entities.AsNoTracking().AnyAsync(entity => entity.Id.Equals(id), cancellationToken);
        }
    }
}
