using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class NestingBoxService : CrudServiceBase<NestingBoxEntity, NestingBox, string>, INestingBoxService
    {
        protected override DbSet<NestingBoxEntity> Entities => DbContext.NestingBoxes;

        public NestingBoxService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
