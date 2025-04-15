using AutoMapper;
using E_commerce.Application.Application;

namespace E_commerce.Infrastructure.Mappings
{
    public class RoleMappingProfile : Profile, IMapFrom
    {
        //Domain to DB
        public RoleMappingProfile(){

            //Domain to DB
            CreateMap<Core.Entities.Role, Data.Models.Role>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.role_id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.role_name))
                .ForMember(dest => dest.Describe, opt => opt.MapFrom(src => src.describe));

            //DB to domain
            CreateMap<Data.Models.Role, Core.Entities.Role>()
                .ForMember(dest => dest.role_id, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.role_name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.describe, opt => opt.MapFrom(src => src.Describe));
        }
    }
}