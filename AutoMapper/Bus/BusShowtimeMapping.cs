using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class BusShowtimeMapping : Profile
    {
        public BusShowtimeMapping()
        {
            CreateMap<BusShowtime, BusShowtimeResponse>()
                .ForMember(d => d.CompanyVehicleId, opt => opt.MapFrom(s => s.CompanyVehicleId))
                .ForMember(d => d.PlateNumber, opt => opt.MapFrom(s => s.CompanyVehicle.PlateNumber))
                .ForMember(d => d.CompanyDriverId, opt => opt.MapFrom(s => s.CompanyDriverId))
                .ForMember(d => d.DriverName, opt => opt.MapFrom(s => s.CompanyDriver.User.FullName))
                .ForMember(d => d.SeatCount, opt => opt.MapFrom(s => s.Seats.Count(x => x.DeleteAt == null)));

            CreateMap<BusShowtime, BusShowtimeSearchResponse>()
                .IncludeBase<BusShowtime, BusShowtimeResponse>()
                .ForMember(d => d.StartPoint, opt => opt.MapFrom(s =>
                    s.Route.RoutePoints
                        .Where(rp => rp.DeleteAt == null && rp.StopType == AppGroupCar.Entities.RouteStopType.Start)
                        .Select(rp => rp.Location.Name)
                        .FirstOrDefault() ?? string.Empty))
                .ForMember(d => d.EndPoint, opt => opt.MapFrom(s =>
                    s.Route.RoutePoints
                        .Where(rp => rp.DeleteAt == null && rp.StopType == AppGroupCar.Entities.RouteStopType.End)
                        .Select(rp => rp.Location.Name)
                        .FirstOrDefault() ?? string.Empty))
                .ForMember(d => d.RatingCompanyAverage, opt => opt.MapFrom(s => s.Company.RatingCompanyAverage))
                .ForMember(d => d.RatingCompanyCount, opt => opt.MapFrom(s => s.Company.RatingCompanyCount))
                .ForMember(d => d.EstimatedDurationMinutes, opt => opt.MapFrom(s => s.Route.EstimatedDurationMinutes));
        }
    }
}
