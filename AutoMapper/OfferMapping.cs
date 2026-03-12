using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.AutoMapper
{
    public class OfferMapping : Profile
    {
        public OfferMapping()
        {
            CreateMap<CreateOfferRequestDTO, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateOfferRequestDTO, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Offer, GetOfferDTO>()
                // Ánh x? DriverName t? thu?c tính Name c?a b?ng Driver
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Name : string.Empty))
                // Ánh x? VehicleName t? thu?c tính Brand c?a b?ng Vehicle
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Brand : string.Empty))
                // Ánh x? TotalSeats t? b?ng SharedRideDetail (n?u có)
                .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.SharedRideDetail != null ? src.SharedRideDetail.TotalSeats : (int?)null))
                // Ánh x? AvailableSeats t? b?ng SharedRideDetail (n?u có)
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.SharedRideDetail != null ? src.SharedRideDetail.AvailableSeats : (int?)null));
        }
    }
}
