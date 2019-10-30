using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class RegionService : CrudServiceBase<RegionEntity, Region, int>, IRegionService
    {
        public RegionService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
