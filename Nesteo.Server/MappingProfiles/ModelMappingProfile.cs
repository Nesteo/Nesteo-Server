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
            CreateMap<NestingBoxEntity, NestingBoxExportRow>().ForMember(dest => dest.Region,
                                                                         options => options.MapFrom(nestingBox => nestingBox.Region.Name))
                                                              .ForMember(dest => dest.Owner,
                                                                         options => options.MapFrom(nestingBox => nestingBox.Owner.Name));
            CreateMap<InspectionEntity, Inspection>().ForMember(dest => dest.HasImage, options => options.MapFrom(inspection => inspection.ImageFileName != null));
            CreateMap<InspectionEntity, InspectionPreview>();
            CreateMap<InspectionEntity, InspectionExportRow>().ForMember(dest => dest.InspectedByUser,
                                                                         options => options.MapFrom(inspection => inspection.InspectedByUser.UserName))
                                                              .ForMember(dest => dest.FemaleParentBirdDiscovery,
                                                                         options => options.MapFrom(inspections => inspections.FemaleParentBirdDiscovery))
                                                              .ForMember(dest => dest.MaleParentBirdDiscovery,
                                                                         options => options.MapFrom(inspections => inspections.MaleParentBirdDiscovery));
        }
    }
}
