using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class OwnerService : CrudServiceBase<OwnerEntity, Owner, int?>, IOwnerService
    {
        public OwnerService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
