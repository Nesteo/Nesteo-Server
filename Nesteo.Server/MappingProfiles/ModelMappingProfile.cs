using AutoMapper;
using Nesteo.Server.Data.Entities;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Models;

namespace Nesteo.Server.MappingProfiles
{
    public class ModelMappingProfile : Profile
    {
        public ModelMappingProfile()
        {
            CreateMap<UserEntity, User>();
            CreateMap<RegionEntity, Region>();
            CreateMap<OwnerEntity, Owner>();
            CreateMap<SpeciesEntity, Species>();
            CreateMap<NestingBoxEntity, NestingBox>();
            CreateMap<InspectionEntity, Inspection>();
        }
    }
}
