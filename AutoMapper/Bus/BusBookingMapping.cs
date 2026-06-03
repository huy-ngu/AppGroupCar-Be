using AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class BusBookingMapping : Profile
    {
        public BusBookingMapping()
        {
            CreateMap<BusBooking, BusBookingResponseDto>()
                .ForMember(d => d.BookingId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.DepartureDate, opt => opt.MapFrom(s => s.Showtime.DepartureDate))
                .ForMember(d => d.DepartureTime, opt => opt.MapFrom(s => s.Showtime.DepartureTime))
                .ForMember(d => d.SeatCount, opt => opt.MapFrom(s => s.Seats.Count))
                .ForMember(d => d.StartPoint, opt => opt.MapFrom(s => GetStartPoint(s.Showtime.Route)))
                .ForMember(d => d.EndPoint, opt => opt.MapFrom(s => GetEndPoint(s.Showtime.Route)));

            CreateMap<BusBookingSeat, BusBookingSeatResponseDto>();
        }

        private BusRoutePointDto GetStartPoint(BusRoute route)
        {
            var startPoint = route.RoutePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.Start);
            return startPoint == null ? null : new BusRoutePointDto
            {
                LocationId = startPoint.LocationId,
                LocationName = startPoint.Location.Name,
                StopType = startPoint.StopType.ToString(),
                Sequence = startPoint.Sequence
            };
        }

        private BusRoutePointDto GetEndPoint(BusRoute route)
        {
            var endPoint = route.RoutePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.End);
            return endPoint == null ? null : new BusRoutePointDto
            {
                LocationId = endPoint.LocationId,
                LocationName = endPoint.Location.Name,
                StopType = endPoint.StopType.ToString(),
                Sequence = endPoint.Sequence
            };
        }
    }
}

