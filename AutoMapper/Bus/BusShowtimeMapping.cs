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
        }
    }
}
