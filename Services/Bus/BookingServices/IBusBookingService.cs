using AppGroupCar.DTO.RequestDTO.Bus.BookingRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse;

namespace AppGroupCar.Services.Bus.BookingServices
{
    public interface IBusBookingService
    {
        Task<BusBookingResponseDto> CreateAsync(Guid userId, CreateBusBookingRequest dto);
        Task<List<BusBookingResponseDto>> GetMyBookingsAsync(Guid userId);
        Task<BusBookingResponseDto> GetByIdAsync(Guid bookingId, Guid userId);
    }
}
