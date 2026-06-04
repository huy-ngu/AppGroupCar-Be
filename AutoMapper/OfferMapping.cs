using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities;
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

            CreateMap<CreateOfferShipmentRequestDTO, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateOfferSharedRidelRequestDTO, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateOfferRequestDTO, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Offer, GetOfferDTO>()
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Brand))
                .ForMember(dest => dest.VehicleUrlImage, opt => opt.MapFrom(src => src.Vehicle.UrlImage))
                .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.Vehicle.PlateNumber))
                .ForMember(dest => dest.StartPoint, opt => opt.MapFrom(src => src.RoutePoints
                    .Where(rp => rp.StopType == RouteStopType.Start)
                    .Select(rp => new LocationDetailDTO
                    {
                        LocationId = rp.LocationId,
                        LocationName = rp.Location.Name
                    })
                    .FirstOrDefault()))
                .ForMember(dest => dest.EndPoint, opt => opt.MapFrom(src => src.RoutePoints
                    .Where(rp => rp.StopType == RouteStopType.End)
                    .Select(rp => new LocationDetailDTO
                    {
                        LocationId = rp.LocationId,
                        LocationName = rp.Location.Name
                    })
                    .FirstOrDefault()));

            CreateMap<Offer, GetOfferSharedRideDTO>()
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Brand))
                .ForMember(dest => dest.VehicleUrlImage, opt => opt.MapFrom(src => src.Vehicle.UrlImage))
                .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.Vehicle.PlateNumber))
                .ForMember(dest => dest.StartPoint, opt => opt.Ignore())
                .ForMember(dest => dest.EndPoint, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.SharedRideDetail.AvailableSeats));

            CreateMap<Offer, GetOfferShipmentDTO>()
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Brand))
                .ForMember(dest => dest.VehicleUrlImage, opt => opt.MapFrom(src => src.Vehicle.UrlImage))
                .ForMember(dest => dest.StartPoint, opt => opt.Ignore())
                .ForMember(dest => dest.EndPoint, opt => opt.Ignore())
                .ForMember(dest => dest.CargoDetail, opt => opt.MapFrom(src => src.CargoDetail));

            CreateMap<CargoOfferDetail, CargoDetailDTO>();

            CreateMap<OfferRoutePoint, GetOfferRoutePointDTO>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.Name));

        }
    }
}
