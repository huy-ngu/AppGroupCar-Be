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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseDocumentUrl, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateDriverRequestDTO, Driver>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseDocumentUrl, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
