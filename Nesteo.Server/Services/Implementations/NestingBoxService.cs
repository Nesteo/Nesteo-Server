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
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class NestingBoxService : CrudServiceBase<NestingBoxEntity, NestingBox, string>, INestingBoxService
    {
        public NestingBoxService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }

        public IAsyncEnumerable<NestingBoxPreview> GetAllPreviewsAsync()
        {
            return Entities.AsQueryable().OrderBy(entity => entity.Id).ProjectTo<NestingBoxPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<NestingBoxPreview> FindPreviewByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return Entities.AsQueryable().Where(entity => entity.Id == id).ProjectTo<NestingBoxPreview>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<string> GetAllTakenIdsAsync()
        {
            return Entities.AsQueryable().OrderBy(entity => entity.Id).Select(entity => entity.Id).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<string> GetAllTakenIdsInRegionAsync(int regionId)
        {
            return Entities.AsQueryable().Where(entity => entity.Region.Id == regionId).OrderBy(entity => entity.Id).Select(entity => entity.Id).AsAsyncEnumerable();
        }
    }
}
