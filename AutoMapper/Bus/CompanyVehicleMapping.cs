using AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class CompanyVehicleMapping : Profile
    {
        public CompanyVehicleMapping()
        {
            CreateMap<CreateCompanyVehicleRequest, CompanyVehicle>()
                .ForMember(d => d.CompanyVehicleId, opt => opt.Ignore())
                .ForMember(d => d.CompanyId, opt => opt.Ignore())
                .ForMember(d => d.UrlImage, opt => opt.Ignore())
                .ForMember(d => d.RegistrationDocumentUrl, opt => opt.Ignore());

            CreateMap<UpdateCompanyVehicleRequest, CompanyVehicle>()
                .ForMember(d => d.CompanyVehicleId, opt => opt.Ignore())
                .ForMember(d => d.CompanyId, opt => opt.Ignore())
                .ForMember(d => d.PlateNumber, opt => opt.Ignore())
                .ForMember(d => d.UrlImage, opt => opt.Ignore())
                .ForMember(d => d.RegistrationDocumentUrl, opt => opt.Ignore());

            CreateMap<CompanyVehicle, CompanyVehicleResponseDto>();
            CreateMap<CompanyVehicle, CompanyVehicleDetailResponseDto>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.CompanyName))
                .ForMember(d => d.SeatLayoutName, opt => opt.MapFrom(s => s.VehicleSeatLayout.SeatLayoutName))
                .ForMember(d => d.SeatLayoutDescription, opt => opt.MapFrom(s => s.VehicleSeatLayout.Description))
                .ForMember(d => d.SeatLayoutIsActive, opt => opt.MapFrom(s => s.VehicleSeatLayout.IsActive));
        }
    }
}
