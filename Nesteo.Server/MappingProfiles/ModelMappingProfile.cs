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
            CreateMap<UserEntity, User>().ReverseMap();
            CreateMap<RegionEntity, Region>().ReverseMap();
            CreateMap<OwnerEntity, Owner>().ReverseMap();
            CreateMap<SpeciesEntity, Species>().ReverseMap();
            CreateMap<NestingBoxEntity, NestingBox>().ForMember(dest => dest.InspectionsCount, options => options.MapFrom(nestingBox => nestingBox.Inspections.Count)).ForMember(
                                                         dest => dest.LastInspected,
                                                         options => options.MapFrom(nestingBox => nestingBox
                                                                                                  .Inspections.OrderByDescending(inspection => inspection.InspectionDate)
                                                                                                  .FirstOrDefault().InspectionDate))
                                                     .ForMember(dest => dest.HasImage, options => options.MapFrom(nestingBox => nestingBox.ImageFileName != null));
            CreateMap<NestingBoxEntity, NestingBoxPreview>().ForMember(dest => dest.InspectionsCount, options => options.MapFrom(nestingBox => nestingBox.Inspections.Count))
                                                            .ForMember(dest => dest.LastInspected,
                                                                       options => options.MapFrom(nestingBox => nestingBox
                                                                                                                .Inspections
                                                                                                                .OrderByDescending(inspection => inspection.InspectionDate)
                                                                                                                .FirstOrDefault().InspectionDate));
            CreateMap<InspectionEntity, Inspection>().ForMember(dest => dest.HasImage, options => options.MapFrom(inspection => inspection.ImageFileName != null));
            CreateMap<InspectionEntity, InspectionPreview>();
        }
    }
}
