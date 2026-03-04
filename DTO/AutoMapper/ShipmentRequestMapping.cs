using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.DTO.AutoMapper
{
    public class ShipmentRequestMapping : Profile
    {
        public ShipmentRequestMapping()
        {
            // dành cho Guid
            CreateMap<UpdateShipmentRequestDTO, ShipmentRequest>()
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Condition(src => src.DropoffLocationId != null))
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Condition(src => src.PickupLocationId != null))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            // mặc định
            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            // Bỏ qua thuộc tính imgUrl 
            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
