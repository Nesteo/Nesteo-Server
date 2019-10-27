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
    public class InspectionService : CrudServiceBase<InspectionEntity, Inspection, int>, IInspectionService
    {
        protected override DbSet<InspectionEntity> Entities => DbContext.Inspections;

        public InspectionService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }

        public IAsyncEnumerable<InspectionPreview> GetAllPreviewsAsync()
        {
            return Entities.ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<InspectionPreview> FindPreviewByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Entities.Where(entity => entity.Id.Equals(id)).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<Inspection> FindByNestingBoxIdAsync(string id)
        {
           return Entities.Where(entity => entity.NestingBox.Id.Equals(id)).ProjectTo<Inspection>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }
    }
}
