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
        public InspectionService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }

        public IAsyncEnumerable<InspectionPreview> GetAllPreviewsAsync()
        {
            return Entities.OrderBy(entity => entity.Id).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public Task<InspectionPreview> FindPreviewByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Entities.Where(entity => entity.Id == id).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<Inspection> GetAllForNestingBoxIdAsync(string nestingBoxId)
        {
            return Entities.OrderBy(entity => entity.Id).Where(entity => entity.NestingBox.Id == nestingBoxId).ProjectTo<Inspection>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<InspectionPreview> GetAllPreviewsForNestingBoxIdAsync(string nestingBoxId)
        {
            return Entities.OrderBy(entity => entity.Id).Where(entity => entity.NestingBox.Id == nestingBoxId).ProjectTo<InspectionPreview>(Mapper.ConfigurationProvider).AsAsyncEnumerable();
        }
    }
}
