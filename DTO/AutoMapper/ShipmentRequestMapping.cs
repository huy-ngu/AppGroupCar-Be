using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.DTO.AutoMapper
{
    public class ShipmentRequestMapping : Profile
    {
        public ShipmentRequestMapping()
        {
            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateShipmentRequestDTO, ShipmentRequest>()
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Condition(src => src.DropoffLocationId != null))
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Condition(src => src.PickupLocationId != null));
            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
