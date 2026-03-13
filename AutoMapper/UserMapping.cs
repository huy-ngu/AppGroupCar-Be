using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.AutoMapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // response
            CreateMap<ApplicationUser, UserDto>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // create
            CreateMap<CreateUserDTO, ApplicationUser>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //update
            CreateMap<UpdateUserDTO, ApplicationUser>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());
        }
    }
}
