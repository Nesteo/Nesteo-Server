using AutoMapper;
using Nesteo.Server.Data.Identity;
using Nesteo.Server.Models;

namespace Nesteo.Server.MappingProfiles
{
    public class ModelMappingProfile : Profile
    {
        public ModelMappingProfile()
        {
            CreateMap<NesteoUser, User>();
        }
    }
}
