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
            // create
            CreateMap<ApplicationUser, UserDto>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateUserDTO, ApplicationUser>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateUserDTO, ApplicationUser>().ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

            //update
            CreateMap<UpdateUserDTO, ApplicationUser>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());
        }
    }
}
