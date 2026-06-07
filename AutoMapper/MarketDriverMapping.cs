using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AutoMapper;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.DTO.ResponseDTO.Market;

namespace AppGroupCar.AutoMapper
{
    public class MarketDriverMapping : Profile
    {
        public MarketDriverMapping()
        {
            CreateMap<CreateDriverRequestDTO, Driver>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateDriverRequestDTO, Driver>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Driver, GetMarketDriverDetailDTO>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<Driver, GetMarketDriverMeDTO>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}
