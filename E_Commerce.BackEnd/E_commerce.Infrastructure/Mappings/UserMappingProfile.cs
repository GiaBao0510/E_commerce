using AutoMapper;
using E_commerce.Application.Application;

namespace E_commerce.Infrastructure.Mappings 
{
    public class UserMappingProfile: Profile, IMapFrom
    {
        public UserMappingProfile(){

            //Domain to Db
            CreateMap<Core.Entities._User, Data.Models.User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.user_id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.user_name))
                .ForMember(dest => dest.PassWord, opt => opt.MapFrom(src => src.pass_word))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.PhoneNum, opt => opt.MapFrom(src => src.phone_num))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.date_of_birth))
                .ForMember(dest => dest.IsBlock, opt => opt.MapFrom(src => src.is_block))
                .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => src.is_delete));

            //DB to Domain
            CreateMap<Data.Models.User, Core.Entities._User>()
                .ForMember(dest => dest.user_id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.user_name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.pass_word, opt => opt.MapFrom(src => src.PassWord))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.phone_num, opt => opt.MapFrom(src => src.PhoneNum))
                .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.date_of_birth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.is_block, opt => opt.MapFrom(src => src.IsBlock))
                .ForMember(dest => dest.is_delete, opt => opt.MapFrom(src => src.IsDelete));
        }
    }
}