using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class OwnerService : CrudServiceBase<OwnerEntity, Owner, int>, IOwnerService
    {
        protected override DbSet<OwnerEntity> Entities => DbContext.Owners;

        public OwnerService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
