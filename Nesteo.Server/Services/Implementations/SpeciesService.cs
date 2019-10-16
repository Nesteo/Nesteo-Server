using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class SpeciesService : CrudServiceBase<SpeciesEntity, Species, int>, ISpeciesService
    {
        protected override DbSet<SpeciesEntity> Entities => DbContext.Species;

        public SpeciesService(NesteoDbContext dbContext, IMapper mapper) : base(dbContext, mapper) { }
    }
}
