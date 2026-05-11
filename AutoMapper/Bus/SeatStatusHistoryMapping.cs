using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class SeatStatusHistoryMapping : Profile
    {
        public SeatStatusHistoryMapping()
        {
            CreateMap<SeatStatusHistory, SeatStatusHistoryResponseDto>()
                .ForMember(d => d.SeatNumber, opt => opt.MapFrom(s => s.Seat.SeatNumber));
        }
    }
}
