using AppGroupCar.DTO.RequestDTO.Market.Vehicle;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.AutoMapper
{
    public class VehicleMapping : Profile
    {
        public VehicleMapping()
        {
            CreateMap<CreateVehicleDTO, Vehicle>()
                .ForMember(dest => dest.UrlImage, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDocumentUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => VehicleStatus.Active));

            CreateMap<UpdateVehicleDTO, Vehicle>()
                .ForMember(dest => dest.UrlImage, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDocumentUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DriverId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Vehicle, VehicleDTO>();
        }
    }
}
