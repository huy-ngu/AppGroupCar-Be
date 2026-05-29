using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;

namespace AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse
{
    public class BusBookingDetailResponseDto : BusBookingResponseDto
    {
        public BusShowtimeDetailResponseDto Showtime { get; set; } = new();
    }
}
