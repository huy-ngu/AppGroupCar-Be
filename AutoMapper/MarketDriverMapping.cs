using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AutoMapper;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;

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

            CreateMap<CreateDriverRequestDTO, ShipmentRequest>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateDriverRequestDTO, ShipmentRequest>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
