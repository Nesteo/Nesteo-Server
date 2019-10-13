using System.Linq;
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
            CreateMap<NestingBoxEntity, NestingBox>().ForMember(dest => dest.InspectionsCount, options => options.MapFrom(nestingBox => nestingBox.Inspections.Count)).ForMember(
                dest => dest.LastInspected,
                options => options.MapFrom(nestingBox => nestingBox
                                                         .Inspections.OrderByDescending(inspection => inspection.InspectionDate)
                                                         .FirstOrDefault().InspectionDate));
            CreateMap<NestingBoxEntity, NestingBoxPreview>().ForMember(dest => dest.InspectionsCount, options => options.MapFrom(nestingBox => nestingBox.Inspections.Count))
                                                            .ForMember(dest => dest.LastInspected,
                                                                       options => options.MapFrom(nestingBox => nestingBox
                                                                                                                .Inspections
                                                                                                                .OrderByDescending(inspection => inspection.InspectionDate)
                                                                                                                .FirstOrDefault().InspectionDate));
            CreateMap<InspectionEntity, Inspection>();
        }
    }
}
