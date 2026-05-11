using AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class BusBookingMapping : Profile
    {
        public BusBookingMapping()
        {
            CreateMap<BusBooking, BusBookingResponseDto>()
                .ForMember(d => d.BookingId, opt => opt.MapFrom(s => s.Id));

            CreateMap<BusBookingSeat, BusBookingSeatResponseDto>();
        }
    }
}
